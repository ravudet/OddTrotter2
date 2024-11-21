////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.V2;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
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

    public interface IOdataContext
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

        public sealed class Error
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
        private OdataErrorResponse()
        {
        }

        //// TODO implement this
        //// do you want to do something AST looking, or somethign with structured properties and things like "provided" markups?
    }

    public interface IOdataClient
    {
        Task<HttpResponseMessage> GetAsync(RelativeUri relativeUri);

        Task<HttpResponseMessage> GetAsync(AbsoluteUri absoluteUri);
    }

    public sealed class OdataCalendarEventsContext : IOdataContext
    {
        private readonly IOdataClient odataClient;

        public OdataCalendarEventsContext(IOdataClient odataClient)
        {
            this.odataClient = odataClient;
        }

        public async Task<OdataCollectionResponse> GetCollection(OdataCollectionRequest request)
        {
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
                    var httpResponseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                    try
                    {
                        httpResponse.EnsureSuccessStatusCode();
                    }
                    catch (HttpRequestException e)
                    {
                        //// TODO throw new GraphException(httpResponseContent, e);
                        throw new Exception(httpResponseContent, e);
                    }

                    var odataCollectionPage = JsonSerializer.Deserialize<OdataCollectionResponseBuilder>(httpResponseContent);
                    if (odataCollectionPage == null)
                    {
                        throw new JsonException($"Deserialized value was null. Serialized value was '{httpResponseContent}'");
                    }

                    if (odataCollectionPage.Value == null)
                    {
                        throw new JsonException($"The value of the collection JSON property was null. The serialized value was '{httpResponseContent}'");
                    }

                    return new OdataCollectionResponse(new OdataCollectionResponse.SpecializedResponse.Collection(
                        odataCollectionPage.Value, 
                        odataCollectionPage.NextLink));
                }
                finally
                {
                    httpResponse?.Dispose();
                }
            }

            throw new Exception("TODO handle requests in a general way");
        }

        private sealed class OdataCollectionResponseBuilder
        {
            [JsonPropertyName("value")]
            public IReadOnlyList<JsonNode>? Value { get; set; }

            [JsonPropertyName("@odata.nextLink")]
            public string? NextLink { get; set; }
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
        internal static async Task<QueryResult<JsonNode, OdataPaginationError>> PageCollection(this IOdataContext odataContext, OdataCollectionRequest request)
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
