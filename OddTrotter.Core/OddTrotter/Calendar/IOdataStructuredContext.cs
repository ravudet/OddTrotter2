////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.V2;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OddTrotter.Calendar
{
    /*public sealed class Filter
    {
        private Filter()
        {
        }
    }

    public sealed class OrderBy
    {
        private OrderBy()
        {
        }
    }

    public sealed class Select
    {
        private Select()
        {
        }
    }

    public sealed class Top
    {
        private Top()
        {
        }
    }

    public interface IOdataCollectionRequestBuilder //// TODO should you have request builders or uri builders? or maybe a builder for each component of the request?
    {
        OdataCollectionRequest Request { get; }

        IOdataCollectionRequestBuilder Filter(Filter filter);

        IOdataCollectionRequestBuilder OrderBy(OrderBy orderBy);

        IOdataCollectionRequestBuilder Select(Select select);

        IOdataCollectionRequestBuilder Top(Top top);
    }*/

    public sealed class OdataCollectionRequest
    {
        /*public OdataCollectionRequest(OdataUri uri)
        {
            this.Uri = uri;
        }

        public OdataUri Uri { get; }*/

        internal OdataCollectionRequest(SpecializedRequest request)
        {
            this.Request = request;
        }

        internal SpecializedRequest Request { get; }

        internal abstract class SpecializedRequest
        {
            private SpecializedRequest()
            {
            }

            public sealed class GetInstanceEvents : SpecializedRequest
            {
                public GetInstanceEvents(DateTime startTime, int pageSize, DateTime endTime)
                {
                    this.StartTime = startTime;
                    this.PageSize = pageSize;
                    this.EndTime = endTime;
                }

                public DateTime StartTime { get; }

                public int PageSize { get; }

                public DateTime EndTime { get; }
            }

            public sealed class GetAbsoluteUri : SpecializedRequest
            {
                public GetAbsoluteUri(AbsoluteUri uri)
                {
                    this.Uri = uri;
                }

                public AbsoluteUri Uri { get; }
            }
        }
    }

    /*public sealed class OdataUri
    {
        public OdataUri(RelativeUri? relativeUri, AbsoluteUri? absoluteUri)
        {
            //// TODO this shouldn't have nullables and stuff, figure out how to handle absolute + relaetive uris

            //// TODO this should have significantly more structure
            this.RelativeUri = relativeUri;
            AbsoluteUri = absoluteUri;
        }

        public RelativeUri? RelativeUri { get; }

        public AbsoluteUri? AbsoluteUri { get; }
    }*/

    public interface IOdataStructuredContext
    {
        Task<OdataCollectionResponse> GetCollection(OdataCollectionRequest request); //// TODO you can get a legal odata response from any url, even ones that are not valid odata urls; maybe you should have an adapter from things like odatacollectionrequest to httprequestmessage?
    }

    /*public sealed class OdataObject
    {
        public OdataObject(JsonNode jsonNode)
        {
            //// TODO this should be more AST-looking instead of using JSON
            //// TODO you've named this type odata "object", but could it also be individual values (like in a collection)?
            this.JsonNode = jsonNode;
        }

        public JsonNode JsonNode { get; }
    }*/

    public abstract class OdataCollectionResponse
    {
        private OdataCollectionResponse()
        {
        }

        //// TODO visitor

        public sealed class Success : OdataCollectionResponse
        {
            //// TODO there should be a more AST-looking intermediate

            [Obsolete]
            internal Success(SpecializedResponse response)
            {
                this.Response = response;
            }

            internal SpecializedResponse Response { get; }

            internal abstract class SpecializedResponse
            {
                public sealed class Collection : SpecializedResponse
                {
                    public Collection(IReadOnlyList<JsonNode> value, string? nextLink)
                    {
                        this.Value = value;
                        this.NextLink = nextLink;
                    }

                    public IReadOnlyList<JsonNode> Value { get; }

                    public string? NextLink { get; }
                }
            }
        }

        public sealed class Error : OdataCollectionResponse
        {
            public Error(OdataErrorResponse odataErrorResponse)
            {
                OdataErrorResponse = odataErrorResponse;
            }

            public OdataErrorResponse OdataErrorResponse { get; }
        }
    }

    public sealed class OdataErrorResponse
    {
        public OdataErrorResponse(string code)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("TODO cannot be empty", nameof(code));
            }

            this.Code = code;
        }

        public string Code { get; }

        //// TODO finish implementing this
    }

    public sealed class OdataDeserializationException : Exception
    {
        public OdataDeserializationException(string message)
            : base(message)
        {
        }

        public OdataDeserializationException(string message, Exception e)
            : base(message, e)
        {
            //// TODO other overloads
        }
    }

    public struct Void
    {
    }

    public sealed class OdataCalendarEventsContext : IOdataStructuredContext
    {
        private readonly IOdataClient odataClient;

        public OdataCalendarEventsContext(IOdataClient odataClient)
        {
            this.odataClient = odataClient;
        }

        public async Task<OdataCollectionResponse> GetCollection(OdataCollectionRequest request)
        {
            //// TODO use Either for the return type instead of a DU for the response type?
            if (request.Request is OdataCollectionRequest.SpecializedRequest.GetInstanceEvents getInstanceEventsRequest)
            {
                var getInstanceEventsUri = 
                    $"/me/calendar/events?" +
                    $"$select=body,start,subject&" +
                    $"$top={getInstanceEventsRequest.PageSize}&" +
                    $"$orderBy=start/dateTime&" +
                    $"$filter=type eq 'singleInstance' and start/dateTime gt '{getInstanceEventsRequest.StartTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.000000")}' and isCancelled eq false and start/dateTime lt '{getInstanceEventsRequest.EndTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.000000")}";
                var requestUri = new Uri(getInstanceEventsUri, UriKind.Relative).ToRelativeUri();

                HttpResponseMessage? httpResponse = null;
                try
                {
                    httpResponse = await this.odataClient.GetAsync(requestUri).ConfigureAwait(false);
                    try
                    {
                        httpResponse.EnsureSuccessStatusCode();
                    }
                    catch (HttpRequestException)
                    {
                        OdataErrorResponseBuilder? odataErrorResponseBuilder;
                        try
                        {
                            odataErrorResponseBuilder = JsonSerializer.Deserialize<OdataErrorResponseBuilder>(await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false));
                        }
                        catch (JsonException jsonException)
                        {
                            throw new OdataDeserializationException("TODO", jsonException); //// TODO should there be a separate exception for errors occurring when deserializing an odata error vs and odata success?
                        }

                        //// TODO replace these lines with buildorthrow call?
                        if (odataErrorResponseBuilder == null)
                        {
                            throw new OdataDeserializationException("TODO");
                        }

                        var odataErrorResponse = odataErrorResponseBuilder.Build().ThrowRight();
                        //// TODO should this be an exception?
                        //// TODO how are going to preserve the http status code?
                        //// TODO if you don't use the httprequestexception to prserve the status code, then use issuccessstatuscode instead
                        return new OdataCollectionResponse.Error(odataErrorResponse);
                    }

                    OdataCollectionResponseBuilder? odataCollectionResponseBuilder;
                    try
                    {
                        odataCollectionResponseBuilder = JsonSerializer.Deserialize<OdataCollectionResponseBuilder>(await httpResponse.Content.ReadAsStreamAsync().ConfigureAwait(false));
                    }
                    catch (JsonException jsonException)
                    {
                        throw new OdataDeserializationException("TODO", jsonException);
                    }

                    //// TODO replace these lines with builderorthrow call?
                    if (odataCollectionResponseBuilder == null)
                    {
                        throw new OdataDeserializationException("TODO");
                    }

                    var odataCollectionResponse = odataCollectionResponseBuilder.Build().ThrowRight();
                    return odataCollectionResponse;
                }
                finally
                {
                    httpResponse?.Dispose();
                }
            }

            throw new Exception("TODO handle requests in a general way");
        }

        private static T BuildOrThrow<T>(IBuilder<T>? builder)
        {
            if (builder == null)
            {
                throw new OdataDeserializationException("TODO");
            }

            return builder.Build().ThrowRight();
        }

        private interface IBuilder<T>
        {
            Either<T, OdataDeserializationException> Build();
        }

        private sealed class OdataCollectionResponseBuilder
        {
            [JsonPropertyName("value")]
            public IReadOnlyList<JsonNode>? Value { get; set; }

            [JsonPropertyName("@odata.nextLink")]
            public string? NextLink { get; set; }

            public Either<OdataCollectionResponse, OdataDeserializationException> Build()
            {
                if (this.Value == null || this.NextLink == null)
                {
                    return new Either<OdataCollectionResponse, OdataDeserializationException>.Right(new OdataDeserializationException("TODO"));
                }

                var specializedResponse = new OdataCollectionResponse.Success.SpecializedResponse.Collection(this.Value, this.NextLink);
                return new Either<OdataCollectionResponse, OdataDeserializationException>.Left(
                    new OdataCollectionResponse.Success(specializedResponse)); //// TODO you can't use the `either` helper factories because the return type is more general than `success`; should discriminated unions have like a "tobasetype" extension or something? or is this an issue with `either` and covariance?
            }
        }

        private sealed class OdataErrorResponseBuilder
        {
            [JsonPropertyName("code")]
            public string? Code { get; set; }

            //// TODO finish implemting this

            public Either<OdataErrorResponse, OdataDeserializationException> Build()
            {
                if (this.Code == null || string.IsNullOrEmpty(this.Code))
                {
                    return Either.Left<OdataErrorResponse>().Right(new OdataDeserializationException("tODO"));
                }

                return Either.Right<OdataDeserializationException>().Left(new OdataErrorResponse(this.Code));
            }
        }
    }

    public sealed class OdataPaginationError
    {
        //// TODO do this correctly

        internal OdataPaginationError()
        {
        }
    }

    public static class OdataContextExtensions
    {
        internal static async Task<QueryResult<JsonNode, OdataPaginationError>> PageCollection(this IOdataStructuredContext odataContext, OdataCollectionRequest request)
        {
            OdataCollectionResponse page;
            try
            {
                page = await odataContext.GetCollection(request).ConfigureAwait(false);
            }
            catch (Exception)
            {
                //// TODO check exception types?
                return new QueryResult<JsonNode, OdataPaginationError>.Partial(new OdataPaginationError()); //// TODO preserve request and exception in error
            }

            QueryResult<JsonNode, OdataPaginationError> complete;
            string? nextLink;
            if (page.Response is OdataCollectionResponse.SpecializedResponse.Collection collectionPage)
            {
                complete = collectionPage.Value.ToQueryResult<JsonNode, OdataPaginationError>();
                nextLink = collectionPage.NextLink;
            }
            else
            {
                throw new Exception("TODO implement in a general way");
            }

            //// TODO make this lazy once queryresult supports it
            while (true)
            {
                if (nextLink == null)
                {
                    return complete;
                }

                AbsoluteUri nextLinkUri;
                try
                {
                    nextLinkUri = new Uri(nextLink, UriKind.Absolute).ToAbsoluteUri();
                }
                catch (UriFormatException)
                {
                    return new QueryResult<JsonNode, OdataPaginationError>.Partial(new OdataPaginationError()); //// TODO preserve nextlinkn and exception
                }

                try
                {
                    request = new OdataCollectionRequest(new OdataCollectionRequest.SpecializedRequest.GetAbsoluteUri(nextLinkUri));
                    page = await odataContext.GetCollection(request).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    //// TODO check exception types?
                    return new QueryResult<JsonNode, OdataPaginationError>.Partial(new OdataPaginationError()); //// TODO preserve request and exception in error
                }

                if (page.Response is OdataCollectionResponse.SpecializedResponse.Collection anotherCollectionPage)
                {
                    complete = complete.Concat(anotherCollectionPage.Value.ToQueryResult<JsonNode, OdataPaginationError>());
                    nextLink = collectionPage.NextLink;
                }
                else
                {
                    throw new Exception("TODO implement in a general way");
                }
            }
        }
    }
}
