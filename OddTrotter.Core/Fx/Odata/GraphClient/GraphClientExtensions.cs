namespace OddTrotter.GraphClient
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    using Fx.Odata;

    /// <summary>
    /// Extension methods for <see cref="IGraphClient"/>
    /// </summary>
    public static class GraphClientExtensions
    {
        public static async Task<OdataCollection<T>> GetOdataCollection<T>(this IGraphClient graphClient, RelativeUri uri)
        {
            var elements = Enumerable.Empty<T>();

            ODataCollectionPage<T> page;
            try
            {
                page = await GetPage<T>(graphClient, uri).ConfigureAwait(false);
            }
            catch (Exception) //// TODO when (e is HttpRequestException || e is GraphException || e is JsonException)
            {
                ////return new ODataCollection<T>(elements, uri.OriginalString);
                throw;
            }

            elements = elements.Concat(page.Value); //// TODO this is a pivotal line; this means that we are concretely enumerating the entire collection, following all pages; we are doing this because we want to have the "lastrequestedurl"; it *might* make sense to change odatacollection to allow the elements to be lazily evaluated and then set "lastrequestedurl" at the time that an error occurs
            
            var nextLink = page.NextLink;
            while (nextLink != null)
            {
                AbsoluteUri nextLinkUri;
                try
                {
                    nextLinkUri = new Uri(nextLink, UriKind.Absolute).ToAbsoluteUri();
                }
                catch (UriFormatException)
                {
                    return new OdataCollection<T>(elements, nextLink);
                }

                try
                {
                    page = GetPage<T>(graphClient, nextLinkUri).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                catch (Exception)//// TODO when (e is HttpRequestException || e is GraphException || e is JsonException)
                {
                    ////return new OdataCollection<T>(elements, nextLinkUri.OriginalString);
                    throw;
                }

                elements = elements.Concat(page.Value);
                nextLink = page.NextLink;
            }

            return new OdataCollection<T>(elements);
        }

        private static async Task<ODataCollectionPage<T>> GetPage<T>(IGraphClient graphClient, RelativeUri uri)
        {
            using (var httpResponse = await graphClient.GetAsync(uri).ConfigureAwait(false))
            {
                return await ReadPage<T>(httpResponse).ConfigureAwait(false);
            }
        }

        private static async Task<ODataCollectionPage<T>> GetPage<T>(IGraphClient graphClient, AbsoluteUri uri)
        {
            using (var httpResponse = await graphClient.GetAsync(uri).ConfigureAwait(false))
            {
                return await ReadPage<T>(httpResponse).ConfigureAwait(false);
            }
        }

        private static async Task<ODataCollectionPage<T>> ReadPage<T>(HttpResponseMessage httpResponse)
        {
            var httpResponseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                httpResponse.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException)
            {
                //// TODO
                ////throw new GraphException(httpResponseContent, e);
                throw;
            }

            var odataCollectionPage = JsonSerializer.Deserialize<ODataCollectionPage<T>.Builder>(httpResponseContent);
            if (odataCollectionPage == null)
            {
                throw new JsonException($"Deserialized value was null. Serialized value was '{httpResponseContent}'");
            }

            if (odataCollectionPage.Value == null)
            {
                throw new JsonException($"The value of the collection JSON property was null. The serialized value was '{httpResponseContent}'");
            }

            return odataCollectionPage.Build();
        }

        private sealed class ODataCollectionPage<T>
        {
            private ODataCollectionPage(IEnumerable<T> value, string? nextLink)
            {
                this.Value = value;
                this.NextLink = nextLink;
            }

            public IEnumerable<T> Value { get; }

            /// <summary>
            /// Gets the URL of the next page in the collection, <see langword="null"/> if there are no more pages in the collection
            /// </summary>
            public string? NextLink { get; }

            public sealed class Builder
            {
                [JsonPropertyName("value")]
                public IEnumerable<T>? Value { get; set; }

                [JsonPropertyName("@odata.nextLink")]
                public string? NextLink { get; set; }

                /// <summary>
                /// 
                /// </summary>
                /// <returns></returns>
                /// <exception cref="ArgumentNullException">Thrown if <see cref="Value"/> is <see langword="null"/></exception>
                public ODataCollectionPage<T> Build()
                {
                    if (this.Value == null)
                    {
                        throw new ArgumentNullException(nameof(this.Value));
                    }

                    return new ODataCollectionPage<T>(this.Value, this.NextLink);
                }
            }
        }
    }
}
