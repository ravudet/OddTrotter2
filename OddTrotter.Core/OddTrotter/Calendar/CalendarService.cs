namespace OddTrotter.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.V2;
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    using OddTrotter.AzureBlobClient;
    using OddTrotter.GraphClient;
    using OddTrotter.TodoList;

    public sealed class CalendarService
    {
        private readonly IGraphClient graphClient;

        private readonly TimeSpan lookAhead;

        public CalendarService(IGraphClient graphClient)
            : this(graphClient, new CalendarServiceSettings.Builder().Build())
        {
        }

        public CalendarService(IGraphClient graphClient, CalendarServiceSettings settings)
        {
            if (graphClient == null)
            {
                throw new ArgumentNullException(nameof(graphClient));
            }

            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }

            this.graphClient = graphClient;
            this.lookAhead = settings.LookAhead;
        }

        public static Func<DateTime> Now { get; set; } = () => DateTime.UtcNow;

        public async Task<TentativeCalendarResult> RetrieveTentativeCalendar()
        {
            //// TODO refactor commonalities between this and todolistservice
            
            //// TODO leverage a cache?
            var startTime = Now();
            var events = GetEvents(this.graphClient, startTime, startTime + this.lookAhead, 50); //// TODO configure these values
            var allEvents = events.Elements.ToList();
            var notResponsedEvents = allEvents
                .Where(calendarEvent => string.Equals(calendarEvent.ResponseStatus?.Response, "notResponded", StringComparison.OrdinalIgnoreCase));
            //// TODO you need to return the malformed calendar events
            var finalEvents = notResponsedEvents
                .Select(calendarEvent =>
                    calendarEvent.Id == null || calendarEvent.Subject == null || calendarEvent.WebLink == null ?
                        null :
                        new TentativeCalendarEvent(calendarEvent.Id, calendarEvent.Subject, calendarEvent.WebLink))
                .Where(calendarEvent => calendarEvent != null);
            return await Task.FromResult(new TentativeCalendarResult(
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                finalEvents
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                ));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="InvalidAccessTokenException">
        /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the requests
        /// </exception>
        /// <remarks>
        /// The returned <see cref="ODataCollection{CalendarEvent}"/> will have its <see cref="ODataCollection{T}.LastRequestedPageUrl"/> be one of three values:
        /// 1. <see langword="null"/> if no errors occurred retrieve any of the data
        /// 2. The URL of series master entity for which an error occurred while retrieving the instance events
        /// 3. The URL of the nextLink for which an error occurred while retrieving the that URL's page
        /// </remarks>
        private static ODataCollection<CalendarEvent> GetEvents(IGraphClient graphClient, DateTime startTime, DateTime endTime, int pageSize)
        {
            var graphCalendarContext = new GraphCalendarContext(graphClient, new Uri($"/me/calendar", UriKind.Relative).ToRelativeUri());
            var calendarContext = new CalendarContext(graphCalendarContext, startTime, endTime); //// TODO constructor injection
            var query = calendarContext
                .Events
                .OrderBy(calendarEvent => calendarEvent.Start.DateTime)
                .Where(calendarEvent => calendarEvent.IsCancelled == false);
            var queried = query.ToArray(); //// TODO is there an async queryable?

            return new ODataCollection<CalendarEvent>(
                queried.Select(_ => new CalendarEvent()
                {
                    Body = new TodoList.BodyStructure()
                    {
                        Content = _.Body?.Content
                    },
                    Id = _.Id,
                    ResponseStatus = new TodoList.ResponseStatusStructure()
                    {
                        Response = _.ResponseStatus?.Response,
                        Time = _.ResponseStatus?.Time,
                    },
                    Start = new TodoList.TimeStructure()
                    {
                        DateTime = _.Start?.DateTime.ToString(),
                        TimeZone = _.Start?.TimeZone,
                    },
                    Subject = _.Subject,
                    WebLink = _.WebLink,
                }),
                null); //// TODO get the last request page uri
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private static string GetInstanceEventsUrl(DateTime startTime, int pageSize)
        {
            //// TODO make the calendar that's used configurable?
            var url =
                $"/me/calendar/events?" +
                $"$select=body,start,subject,responseStatus,webLink&" +
                $"$top={pageSize}&" +
                $"$orderBy=start/dateTime&" +
                $"$filter=type eq 'singleInstance' and start/dateTime gt '{startTime.ToUniversalTime().ToString("yyyy-MM-ddThh:mm:ss.000000")}' and isCancelled eq false";
            return url;
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

        private sealed class ODataCollection<T>
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
}
