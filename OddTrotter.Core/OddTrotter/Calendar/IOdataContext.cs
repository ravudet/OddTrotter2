﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.V2;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OddTrotter.Calendar
{
    public interface IOdataCollectionRequestBuilder
    {
    }

    public sealed class OdataCollectionRequest
    {
        public OdataCollectionRequest(OdataUri uri)
        {
            this.Uri = uri;
        }

        public OdataUri Uri { get; }
    }

    public sealed class OdataUri
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
    }

    public interface IOdataContext
    {
        Task<OdataCollectionResponse> GetCollection(OdataCollectionRequest request);
    }

    public sealed class OdataObject
    {
        public OdataObject(string id)
        {
            this.Id = id;
        }

        [JsonPropertyName("id")] //// TODO
        public string Id { get; }
    }

    public sealed class OdataCollectionResponse
    {
        //// TODO there should be a more AST-looking intermediate

        public OdataCollectionResponse(IReadOnlyList<OdataObject> value, string? nextLink)
        {
            this.Value = value;
            this.NextLink = nextLink;
        }

        public IReadOnlyList<OdataObject> Value { get; }

        public string? NextLink { get; }
    }

    public interface IOdataClient
    {
        Task<HttpResponseMessage> GetAsync(RelativeUri relativeUri);

        Task<HttpResponseMessage> GetAsync(AbsoluteUri absoluteUri);
    }

    public sealed class GraphCalendarEventsContext : IOdataContext
    {
        private readonly IOdataClient odataClient;

        public GraphCalendarEventsContext(IOdataClient odataClient)
        {
            this.odataClient = odataClient;
        }

        public async Task<OdataCollectionResponse> GetCollection(OdataCollectionRequest request)
        {
            using (var httpResponse = await this.odataClient.GetAsync(request.Uri.RelativeUri!).ConfigureAwait(false)) //// TODO nullable uri
            {
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

                //// TODO nullable
                //// TODO select statement
                return new OdataCollectionResponse(odataCollectionPage.Value.Select(node => JsonSerializer.Deserialize<OdataObject>(node)!).ToList(), odataCollectionPage.NextLink);
            }
        }

        private sealed class OdataCollectionResponseBuilder
        {
            [JsonPropertyName("value")]
            public IReadOnlyList<System.Text.Json.Nodes.JsonNode>? Value { get; set; }

            [JsonPropertyName("@odata.nextLink")]
            public string? NextLink { get; set; }
        }
    }

    public sealed class OdataPaginationError
    {
        //// TODO
    }

    public static class OdataContextExtensions
    {
        public static async Task<QueryResult<OdataObject, OdataPaginationError>> PageCollection(this IOdataContext odataContext, OdataCollectionRequest request)
        {
            OdataCollectionResponse page;
            try
            {
                page = await odataContext.GetCollection(request).ConfigureAwait(false);
            }
            catch (Exception)
            {
                //// TODO check exception types?
                return new QueryResult<OdataObject, OdataPaginationError>.Partial(new OdataPaginationError()); //// TODO preserve request and exception in error
            }

            var complete = page.Value.ToQueryResult<OdataObject, OdataPaginationError>();
            //// TODO make this lazy once queryresult supports it
            while (true)
            {
                if (page.NextLink == null)
                {
                    return complete;
                }

                AbsoluteUri nextLinkUri;
                try
                {
                    nextLinkUri = new Uri(page.NextLink, UriKind.Absolute).ToAbsoluteUri();
                }
                catch (UriFormatException)
                {
                    return new QueryResult<OdataObject, OdataPaginationError>.Partial(new OdataPaginationError()); //// TODO preserve nextlinkn and exception
                }

                try
                {
                    request = new OdataCollectionRequest(new OdataUri(null, nextLinkUri));
                    page = await odataContext.GetCollection(request).ConfigureAwait(false);
                }
                catch (Exception)
                {
                    //// TODO check exception types?
                    return new QueryResult<OdataObject, OdataPaginationError>.Partial(new OdataPaginationError()); //// TODO preserve request and exception in error
                }

                complete = complete.Concat(page.Value.ToQueryResult<OdataObject, OdataPaginationError>());
            }
        }
    }
}
