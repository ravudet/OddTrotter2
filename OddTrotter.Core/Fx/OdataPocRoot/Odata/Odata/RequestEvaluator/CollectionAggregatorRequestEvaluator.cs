////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestEvaluator
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Numerics;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Fx.OdataPocRoot.Odata.Odata.RequestEvaluator.Mixins;

    public sealed class BackendEvaluator
    {
        public BackendEvaluator(string backendUri, IRequestEvaluator evaluator)
        {
            BackendUri = backendUri;
            Evaluator = evaluator;
        }

        public string BackendUri { get; } //// TODO you want to pass through query options and stuff, so you really just want the path portion of the URI, maybe also the schema and domain; are there cases where the backend URI needs specific query options? handling that would likely require parsing the OData specific query options and combining them with the original request

        public IRequestEvaluator Evaluator { get; }
    }

    public sealed class CollectionAggregatorRequestEvaluator : IRequestEvaluator, IGetCollectionMixin
    {
        private readonly IRequestEvaluator defaultEvaluator;

        private readonly IReadOnlyDictionary<string, IEnumerable<BackendEvaluator>> aggregatedEvaluators;

        private readonly int pageSize;

        public CollectionAggregatorRequestEvaluator(
            IRequestEvaluator defaultEvaluator,
            IReadOnlyDictionary<string, IEnumerable<BackendEvaluator>> aggregatedEvaluators,
            int pageSize)
        {
            this.defaultEvaluator = defaultEvaluator;
            this.aggregatedEvaluators = aggregatedEvaluators;
            this.pageSize = pageSize; //// TODO use settings
        }

        public async Task<OdataResponse.Instance> Evaluate(OdataRequest.GetInstance request)
        {
            return await this.defaultEvaluator.Evaluate(request).ConfigureAwait(false);
        }

        public async Task<OdataResponse.Collection> Evaluate(OdataRequest.GetCollection request)
        {
            //// TODO can you somehow leverage the generic implementation? what would it look like for someone to actually implement that?
            return await this.defaultEvaluator.Evaluate(request).ConfigureAwait(false);
        }

        public async Task<OdataResponse<T>.GetCollection> Evaluate<T>(OdataRequest<T>.GetCollection request)
        {
            //// TODO in odatarequest.getcollection, you are likely going to the Uri property to include on the path; if that's what happens, check all of your assumptions in this clss about the use of that URI
            var uriPath = request.Request.Uri.GetComponents(RelativeUriComponents.Path, UriFormat.UriEscaped);
            if (!this.aggregatedEvaluators.TryGetValue(uriPath, out var backendEvaluators))
            {
                return await this.defaultEvaluator.Evaluate(request).ConfigureAwait(false);
            }

            IEnumerable<BackendEvaluator> remainingEvaluators;
            RelativeUri? backendNextLink;
            int backendElementsToSkip;
            if (request.Request.SkipToken == null)
            {
                remainingEvaluators = backendEvaluators;
                backendNextLink = null;
                backendElementsToSkip = 0;
            }
            else
            {
                var skipToken = JsonSerializer.Deserialize<SkipToken>(request.Request.SkipToken);
                if (skipToken == null)
                {
                    throw new Exception("TODO null skiptoken provided");
                }

                backendElementsToSkip = skipToken.ToSkip;

                //// TODO this logic might break a client if a deployment happens, for example:
                //// client requests A
                //// aggregator sends A to evaluator 1
                //// aggregator receives A'
                //// aggregator sends A to evaluator 2
                //// aggregator receives A''
                //// aggregator stitches together A' + A''
                //// client receives A' + A'' with skip token B
                //// 
                //// aggregator deployment occurs, putting evaluator 3 at the beginning of the collection
                ////
                //// client requests B
                //// aggregator sends B to evaluator 4 // ERROR client now doesn't receive anything from 3
                remainingEvaluators = backendEvaluators
                    .SkipWhile(evaluator =>
                        !string.Equals(
                            evaluator.BackendUri, 
                            skipToken.CurrentEvaluator, 
                            StringComparison.OrdinalIgnoreCase)); //// TODO it might make sense to have an evaluator factory method here instead of the evaluator...
                backendNextLink = new Uri(skipToken.NextLink, UriKind.Relative).ToRelativeUri(); 
            }

            var elements = new List<T>(this.pageSize);
            foreach (var backendEvaluator in backendEvaluators)
            {
                var response = await FillPageOrExhaustEvaluator(
                    backendEvaluator,
                    request,
                    backendNextLink,
                    elements,
                    this.pageSize,
                    backendElementsToSkip).ConfigureAwait(false);
                if (elements.Count < this.pageSize)
                {
                    // the page isn't full yet, go to the next evaluator
                    continue;
                }

                string evaluatorNextLink;
                int numberOfElementsAlreadyTaken;
                if (response.NextUnexhaustedPage == null)
                {
                    // the page is full, but it was just barely filled by the current backend evaluator; let's get the
                    // uri for the next evaluator
                    evaluatorNextLink = "TODO make this the URI for the next backend evaluator";
                    numberOfElementsAlreadyTaken = 0;
                }
                else
                {
                    evaluatorNextLink = response.NextUnexhaustedPage;
                    numberOfElementsAlreadyTaken = response.NumberOfElementsAlreadyTakenFromThatPage;
                }

                var skipToken = new SkipToken(
                    backendEvaluator.BackendUri,
                    evaluatorNextLink,
                    numberOfElementsAlreadyTaken);
                var serializedSkipToken = JsonSerializer.Serialize(skipToken);
                var encodedSkipToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(serializedSkipToken));
                var nextLinkRequest = new OdataRequest.GetCollection(
                    request.Request.Uri,
                    request.Request.Expand,
                    request.Request.Filter,
                    request.Request.Select,
                    encodedSkipToken);
                var nextLinkRelativeUri = ToRelativeUri(nextLinkRequest);
                var nextLinkUri = new Uri(new Uri("TODO base URI here"), nextLinkRelativeUri).ToAbsoluteUri();
                new OdataResponse<T>.GetCollection(
                    elements,
                    new OdataResponse<T>.GetCollection.CollectionControlInformation(
                        nextLinkUri,
                        null
                        ));
            }
        }

        private static async Task<
            (
                string? NextUnexhaustedPage,
                int NumberOfElementsAlreadyTakenFromThatPage
            )>
                                    FillPageOrExhaustEvaluator<T>(
                                        BackendEvaluator backendEvaluator, 
                                        OdataRequest<T>.GetCollection incomingRequest, 
                                        RelativeUri? backendNextLink, 
                                        List<T> elements, 
                                        int pageSize, 
                                        int toSkip)
        {
            backendNextLink ??= new Uri(backendEvaluator.BackendUri, UriKind.Relative).ToRelativeUri();
            while (true)
            {
                var response = await GetEvaluatorPage(
                    backendEvaluator,
                    incomingRequest,
                    backendNextLink
                    ).ConfigureAwait(false);

                var pageRemainder = pageSize - elements.Count;
                elements.AddRange(response.Elements.Skip(toSkip).Take(pageRemainder));
                if (elements.Count < pageSize)
                {
                    // the aggregator page isn't full
                    if (response.NextLink == null)
                    {
                        // but the evaluator is exhausted
                        return (null, 0);
                    }

                    //// TODO doing this assumes that the nextlink is always at the same domain + scheme + port + etc
                    var nextLink = new Uri(response.NextLink, UriKind.Absolute);
                    var relativeNextLink = nextLink.GetComponents(
                        UriComponents.PathAndQuery | UriComponents.Fragment, 
                        UriFormat.Unescaped); //// TODO extension for this?
                    backendNextLink = new Uri(relativeNextLink, UriKind.Relative).ToRelativeUri();
                    toSkip = 0;
                }
                else
                {
                    // the aggregator page is full
                    if (toSkip + pageRemainder < response.Elements.Count)
                    {
                        // there are still elements on the most recently requested page
                        return (backendNextLink.OriginalString, toSkip + pageRemainder);
                    }
                    else
                    {
                        // the most recently requested page just barely finished the aggregator page
                        return (response.NextLink, 0);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backendEvaluator"></param>
        /// <param name="incomingRequest"></param>
        /// <param name="backendRequestUri"></param>
        /// <param name="aggregatedElements"></param>
        /// <param name="aggregatorPageSize"></param>
        /// <param name="backendElementsToSkip"></param>
        /// <returns>
        /// TODO you've basically just rebuilt odataresponse{T}.getcollection, do you want to re-use that?
        /// 
        /// Elements - the elements of the page
        /// NextLink - the next link of the page received at backendrequesturi
        /// </returns>
        private static async Task<(IReadOnlyList<T> Elements, string? NextLink)> GetEvaluatorPage<T>(
            BackendEvaluator backendEvaluator,
            OdataRequest<T>.GetCollection incomingRequest, 
            RelativeUri backendRequestUri)
        {
            var backendEvaluatorSkipToken = backendRequestUri
                .GetComponents(RelativeUriComponents.Query, UriFormat.UriEscaped)
                .Split("&", StringSplitOptions.RemoveEmptyEntries)
                .Select(option => option.Split("="))
                .Where(option => string.Equals(option[0], "skiptoken", StringComparison.OrdinalIgnoreCase))
                .Select(option => string.Join(string.Empty, option, 1, option.Length - 1))
                .FirstOrDefault();

            var backendRequest = new OdataRequest.GetCollection(
                backendRequestUri,
                incomingRequest.Request.Expand,
                incomingRequest.Request.Filter,
                incomingRequest.Request.Select,
                backendEvaluatorSkipToken);
            var genericBackendRequest = new OdataRequest<T>.GetCollection(backendRequest);

            var backendResponse = await backendEvaluator
                .Evaluator
                .Evaluate(genericBackendRequest)
                .ConfigureAwait(false);

            return
                (
                    backendResponse.Value,
                    backendResponse.ControlInformation.NextLink?.OriginalString
                );
        }

        private static RelativeUri ToRelativeUri(OdataRequest.GetCollection odataRequest)
        {
            //// TODO make this an extension somewhere?

            var uriBuilder = new StringBuilder();
            uriBuilder.Append(odataRequest.Uri.OriginalString.Trim('/'));

            var queryOptions = new List<string>();
            if (odataRequest.Expand != null)
            {
                queryOptions.Add($"$expand={odataRequest.Expand.ToString()}"); //// TODO have adapters from each query option node to its string
            }

            if (odataRequest.Filter != null)
            {
                queryOptions.Add($"$filter={odataRequest.Filter.ToString()}"); //// TODO have adapters from each query option node to its string
            }

            if (odataRequest.Select != null)
            {
                queryOptions.Add($"$select={odataRequest.Select.ToString()}"); //// TODO have adapters from each query option node to its string
            }

            if (odataRequest.SkipToken != null)
            {
                queryOptions.Add($"$skiptoken={odataRequest.SkipToken.ToString()}"); //// TODO have adapters from each query option node to its string
            }

            if (queryOptions.Count > 0)
            {
                uriBuilder.Append("?");
                uriBuilder.AppendJoin("&", queryOptions);
            }

            return new Uri(uriBuilder.ToString(), UriKind.Relative).ToRelativeUri();
        }

        private sealed class SkipToken
        {
            public SkipToken(string currentEvaluator, string nextLink, int toSkip)
            {
                CurrentEvaluator = currentEvaluator;
                NextLink = nextLink;
                ToSkip = toSkip;
            }

            public string CurrentEvaluator { get; }

            public string NextLink { get; }

            public int ToSkip { get; }
        }
    }
}
