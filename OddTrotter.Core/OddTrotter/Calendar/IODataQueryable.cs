namespace OddTrotter.Calendar
{
    using System;
    using System.Collections.Generic;
    using OddTrotter.AzureBlobClient;
    using System.Net.Http;
    using System.Text.Json;
    using OddTrotter.GraphClient;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

/*
how to generate? .../calendar?$expand=events($filter={fitler})
*/

    public sealed class GraphCalendarContext : IODataInstanceContext
    {
        private readonly IGraphClient graphClient;

        private readonly RelativeUri calendarUri;

        public GraphCalendarContext(IGraphClient graphClient, RelativeUri calendarUri)
        {
            this.graphClient = graphClient;
            this.calendarUri = calendarUri;

            this.Events = new CalendarEventCollectionContext(this.graphClient, this.calendarUri);
        }

        public IODataCollectionContext<GraphCalendarEvent> Events { get; }

        private sealed class CalendarEventCollectionContext : IODataCollectionContext<GraphCalendarEvent>
        {
            private readonly IGraphClient graphClient;

            private readonly RelativeUri eventsUri;

            private readonly string? filter;

            private readonly string? select;

            public CalendarEventCollectionContext(IGraphClient graphClient, RelativeUri calendarUri)
                : this(graphClient, calendarUri, null, null)
            {
            }

            private CalendarEventCollectionContext(IGraphClient graphClient, RelativeUri calendarUri, string? filter, string? select)
            {
                this.graphClient = graphClient;
                this.eventsUri = new Uri(calendarUri.OriginalString.TrimEnd('/') + "/events", UriKind.Relative).ToRelativeUri();
                this.filter = filter;
                this.select = select;
            }

/*
var url =
    $"/me/calendar/events?" +
    $"$select=body,start,subject,responseStatus,webLink&" +
    $"$top={pageSize}&" +
    $"$orderBy=start/dateTime&" +
    $"$filter=type eq 'singleInstance' and start/dateTime gt '{startTime.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss.000000")}' and isCancelled eq false";
*/

            public IODataCollectionContext<GraphCalendarEvent> Filter()
            {
                throw new NotImplementedException();
            }

            public IODataCollectionContext<GraphCalendarEvent> Select<TProperty>(Func<GraphCalendarEvent, TProperty> selector)
            {
                throw new NotImplementedException();
            }

            public ODataCollection<GraphCalendarEvent> Values
            {
                get
                {
                    return GetCollection<GraphCalendarEvent>(this.graphClient, this.eventsUri);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="graphClient"></param>
            /// <param name="uri"></param>
            /// <returns></returns>
            /// <exception cref="InvalidAccessTokenException">
            /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the requests
            /// </exception>
            private static ODataCollection<T> GetCollection<T>(IGraphClient graphClient, RelativeUri uri)
            {
                //// TODO use linq instead of a list
                var elements = new List<T>();
                ODataCollectionPage<T> page;
                try
                {
                    //// TODO would it make sense to have a method like "bool TryGetPage(IGraphClient, RelativeUri, out ODataCollectionPage, out ODataCollection)" ?
                    page = GetPage<T>(graphClient, uri).ConfigureAwait(false).GetAwaiter().GetResult();
                }
                catch (Exception e) when (e is HttpRequestException || e is GraphException || e is JsonException)
                {
                    return new ODataCollection<T>(elements, uri.OriginalString);
                }

                elements.AddRange(page.Value);
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
                        return new ODataCollection<T>(elements, nextLink);
                    }

                    try
                    {
                        page = GetPage<T>(graphClient, nextLinkUri).ConfigureAwait(false).GetAwaiter().GetResult();
                    }
                    catch (Exception e) when (e is HttpRequestException || e is GraphException || e is JsonException)
                    {
                        return new ODataCollection<T>(elements, nextLinkUri.OriginalString);
                    }

                    elements.AddRange(page.Value);
                    nextLink = page.NextLink;
                }

                return new ODataCollection<T>(elements);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="graphClient"></param>
            /// <param name="uri"></param>
            /// <returns></returns>
            /// <exception cref="HttpRequestException">
            /// Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout
            /// </exception>
            /// <exception cref="InvalidAccessTokenException">
            /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the request
            /// </exception>
            /// <exception cref="GraphException">Thrown if graph produced an error while retrieving the page</exception>
            /// <exception cref="JsonException">Thrown if the response content was not a valid OData collection payload</exception>
            private static async Task<ODataCollectionPage<T>> GetPage<T>(IGraphClient graphClient, RelativeUri uri)
            {
                using (var httpResponse = await graphClient.GetAsync(uri).ConfigureAwait(false))
                {
                    return await ReadPage<T>(httpResponse).ConfigureAwait(false); //// TODO add configure await to todolistservice
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="graphClient"></param>
            /// <param name="uri"></param>
            /// <returns></returns>
            /// <exception cref="HttpRequestException">
            /// Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout
            /// </exception>
            /// <exception cref="InvalidAccessTokenException">
            /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the request
            /// </exception>
            /// <exception cref="GraphException">Thrown if graph produced an error while retrieving the page</exception>
            /// <exception cref="JsonException">Thrown if the response content was not a valid OData collection payload</exception>
            private static async Task<ODataCollectionPage<T>> GetPage<T>(IGraphClient graphClient, AbsoluteUri uri)
            {
                using (var httpResponse = await graphClient.GetAsync(uri).ConfigureAwait(false))
                {
                    return await ReadPage<T>(httpResponse).ConfigureAwait(false); //// TODO add configure await to todolistservice
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="httpResponse"></param>
            /// <returns></returns>
            /// <exception cref="GraphException">Thrown if graph produced an error while retrieving the page</exception>
            /// <exception cref="JsonException">Thrown if the response content was not a valid OData collection payload</exception>
            private static async Task<ODataCollectionPage<T>> ReadPage<T>(HttpResponseMessage httpResponse)
            {
                var httpResponseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                try
                {
                    httpResponse.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException e)
                {
                    throw new GraphException(httpResponseContent, e);
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

    public interface IODataInstanceContext
    {
        //// TODO
    }

    public interface IODataCollectionContext<T>
    {
        //// TODO do you want this? IEnumerator<T> GetEnumerator();

        ODataCollection<T> Values { get; }

        IODataCollectionContext<T> Select<TProperty>(Func<T, TProperty> selector);

        IODataCollectionContext<T> Filter();

        //// TODO T this[somekey?] { get; }
    }

    public sealed class ODataCollection<T>
    {
        public ODataCollection(IEnumerable<T> elements)
            : this(elements, null)
        {
        }

        public ODataCollection(IEnumerable<T> elements, string? lastRequestedPageUrl)
        {
            this.Elements = elements;
            this.LastRequestedPageUrl = lastRequestedPageUrl;
        }

        public IEnumerable<T> Elements { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// <see langword="null"/> if there are collection was read to completeness; has a value if reading a next link produced an error, where the value is the nextlink that
        /// produced the error
        /// </remarks>
        public string? LastRequestedPageUrl { get; }
    }
}
