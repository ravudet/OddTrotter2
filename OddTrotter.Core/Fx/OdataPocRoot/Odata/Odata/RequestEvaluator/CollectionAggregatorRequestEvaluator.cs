////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestEvaluator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        public async Task<OdataResponse<T>.GetCollection> Evaluate<T>(OdataRequest<T>.GetCollection incomingRequest)
        {
            var uriPath = incomingRequest.Request.Uri.GetComponents(RelativeUriComponents.Path, UriFormat.UriEscaped);
            if (!this.aggregatedEvaluators.TryGetValue(uriPath, out var backendEvaluators))
            {
                return await this.defaultEvaluator.Evaluate(incomingRequest).ConfigureAwait(false);
            }

            IEnumerable<BackendEvaluator> remainingEvaluators;
            RelativeUri? backendNextLink;
            int backendElementsToSkip;
            if (incomingRequest.Request.SkipToken == null)
            {
                remainingEvaluators = backendEvaluators;
                backendNextLink = null;
                backendElementsToSkip = 0;
            }
            else
            {
                var skipToken = JsonSerializer.Deserialize<SkipToken>(incomingRequest.Request.SkipToken);
                if (skipToken == null)
                {
                    throw new Exception("TODO null skiptoken provided");
                }

                backendElementsToSkip = skipToken.ToSkip;

                remainingEvaluators = backendEvaluators
                    .SkipWhile(evaluator =>
                        !string.Equals(
                            evaluator.BackendUri, 
                            skipToken.CurrentEvaluator, 
                            StringComparison.OrdinalIgnoreCase)); //// TODO it might make sense to have an evaluator factory method here instead of the evaluator...
                backendNextLink = new Uri(skipToken.NextLink, UriKind.Relative).ToRelativeUri(); 
            }

            var elements = new List<T>(this.pageSize);
            using (var enumerator = remainingEvaluators.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new Exception("TODO no more evaluators, but at least one is expeceted...");
                }

                var backendEvaluator = enumerator.Current;
                var response = await ExhaustEvaluator(backendEvaluator, incomingRequest, backendNextLink, elements, this.pageSize, backendElementsToSkip).ConfigureAwait(false);
                if (elements.Count < this.pageSize)
                {
                    //// TODO next evaluator, or return if it's the last evaluator
                }
                else
                {
                    var toSkip = response.PageCount - response.Taken;
                    string skipTokenNextLink;
                    if (toSkip > 0)
                    {
                        if (backendNextLink == null)
                        {
                            skipTokenNextLink = backendEvaluator.BackendUri;
                        }
                        else
                        {
                            skipTokenNextLink = backendNextLink.OriginalString;
                        }
                    }
                    else
                    {
                        throw new Exception("TODO next page of current evaluator, or next evalutor");
                    }

                    var skipToken = new SkipToken(
                        backendEvaluator.BackendUri,
                        skipTokenNextLink,
                        toSkip);
                    return new OdataResponse<T>.GetCollection(
                        elements,
                        new OdataResponse<T>.GetCollection.CollectionControlInformation(
                            response.NewNextLink == null ? null : new Uri(response.NewNextLink, UriKind.Absolute).ToAbsoluteUri(),
                            pageSize //// TODO you should only include this if "$count=true"
                        ));
                }
            }
        }

        private static async Task<(string? NewNextLink, int PageCount, int Taken)> ExhaustEvaluator<T>(BackendEvaluator backendEvaluator, OdataRequest<T>.GetCollection incomingRequest, RelativeUri? backendNextLink, List<T> elements, int pageSize, int toSkip)
        {
            var originalCount = elements.Count;
            var response = await GetPage(
                    backendEvaluator,
                    incomingRequest,
                    backendNextLink ?? new Uri(backendEvaluator.BackendUri, UriKind.Relative).ToRelativeUri(),
                    elements,
                    pageSize,
                    toSkip).ConfigureAwait(false);
            if (elements.Count < pageSize)
            {
                await GetPage(
                    backendEvaluator,
                    incomingRequest,
                    )
            }
            else
            {
                
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
        /// Count - the nubmer of elements of the page received at backenduri
        /// </returns>
        private static async Task<(IReadOnlyList<T> Elements, string? NextLink, int Count)> GetPage<T>(
            BackendEvaluator backendEvaluator,
            OdataRequest<T>.GetCollection incomingRequest, 
            RelativeUri backendRequestUri,
            List<T> aggregatedElements,
            int aggregatorPageSize,
            int backendElementsToSkip)
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
                    backendResponse.ControlInformation.NextLink?.OriginalString, 
                    backendResponse.Value.Count
                );
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
