////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using OddTrotter.GraphClient;

    public sealed class OdataError //// TODO graph error?
    {
        public OdataError(string requestedUrl, Exception exception)
        {
            this.RequestedUrl = requestedUrl;
            this.Exception = exception;
        }

        public string RequestedUrl { get; }

        public Exception Exception { get; }
    }

    public sealed class CalendarEventContext : IQueryContext<Either<CalendarEvent, CalendarEventBuilder>, OdataError>
    {
        private readonly IGraphClient graphClient;
        private readonly RelativeUri calendarUri;
        private readonly DateTime startTime;
        private readonly DateTime endTime;
        private readonly int pageSize;

        public CalendarEventContext(
            IGraphClient graphClient, 
            RelativeUri calendarUri, 
            DateTime startTime, 
            DateTime endTime, 
            CalendarEventContextSettings settings)
        {
            this.graphClient = graphClient;
            this.calendarUri = calendarUri; //// TODO you actually just want the path portion
            this.startTime = startTime; //// TODO does datetime make sense for this?
            this.endTime = endTime; //// TODO does datetime make sense for this?
            this.pageSize = settings.PageSize;
        }

        public QueryResult<Either<CalendarEvent, CalendarEventBuilder>, OdataError> Evaluate()
        {
            //// TODO finish implementing this class
            //// TODO write tests for todolistservice that confirm the URLs
            //// TODO convert todolistservice to use this class
            //// TODO update this class to try using odataquerybuilder, odatarequestevaluator, etc; or maybe try adding the pending calendar events stuff first, and then update this class to make it easier to share code
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the requests
        /// </exception>
        /// <remarks>
        /// The returned <see cref="ODataCollection{CalendarEvent}"/> will have its <see cref="ODataCollection{T}.LastRequestedPageUrl"/> be one of three values:
        /// 1. <see langword="null"/> if no errors occurred retrieve any of the data
        /// 2. The URL of series master entity for which an error occurred while retrieving the instance events
        /// 3. The URL of the nextLink for which an error occurred while retrieving the that URL's page
        /// </remarks>
        private static async Task<QueryResult<Either<CalendarEvent, GraphCalendarEvent>, OdataError>> GetEvents(IGraphClient graphClient, DateTime startTime, DateTime endTime, int pageSize)
        {
            var instanceEvents = await GetInstanceEvents(graphClient, startTime, endTime, pageSize).ConfigureAwait(false);
            var seriesEvents = GetSeriesEvents(graphClient, startTime, endTime, pageSize);
            //// TODO merge the sorted sequences instead of concat
            /*return new ODataCollection<CalendarEvent>(
                instanceEvents.Elements.Concat(seriesEvents.Elements),
                instanceEvents.LastRequestedPageUrl ?? seriesEvents.LastRequestedPageUrl);*/

            return instanceEvents;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the requests
        /// </exception>
        private static async Task<QueryResult<Either<CalendarEvent, GraphCalendarEvent>, OdataError>> GetInstanceEvents(IGraphClient graphClient, DateTime startTime, DateTime endTime, int pageSize)
        {
            //// TODO starttime and endtime should be done through a queryable
            var url = GetInstanceEventsUrl(startTime, pageSize) + $" and start/dateTime lt '{endTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.000000")}'";
            var graphCalendarEvents = await GetQueryResult<GraphCalendarEvent>(graphClient, new Uri(url, UriKind.Relative).ToRelativeUri()).ConfigureAwait(false);
            var calendarEvents = await graphCalendarEvents.Select(graphCalendarEvent => graphCalendarEvent.Build()).ConfigureAwait(false);
            return calendarEvents;
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
                $"$select=body,start,subject&" +
                $"$top={pageSize}&" +
                $"$orderBy=start/dateTime&" +
                $"$filter=type eq 'singleInstance' and start/dateTime gt '{startTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.000000")}' and isCancelled eq false";
            return url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the requests
        /// </exception>
        /// <remarks>
        /// The returned <see cref="ODataCollection{CalendarEvent}"/> will have its <see cref="ODataCollection{T}.LastRequestedPageUrl"/> be one of three values:
        /// 1. <see langword="null"/> if no errors occurred retrieve any of the data
        /// 2. The URL of series master entity for which an error occurred while retrieving the instance events
        /// 3. The URL of the nextLink for which an error occurred while retrieving the that URL's page
        /// </remarks>
        private static async Task<QueryResult<CalendarEvent, (string, Exception)>> GetSeriesEvents(IGraphClient graphClient, DateTime startTime, DateTime endTime, int pageSize)
        {
            var seriesEventMasters = await GetSeriesEventMasters(graphClient, pageSize).ConfigureAwait(false);
            //// TODO you could actually filter by subject here before making further requests
            var seriesInstanceEvents = await seriesEventMasters
                .Select(series => (series, GetFirstSeriesInstanceInRange(graphClient, series, startTime, endTime).ConfigureAwait(false).GetAwaiter().GetResult())).ConfigureAwait(false); //// TODO don't use getawaiter...

            return Adapt(seriesInstanceEvents);




            var seriesInstanceEventsWithFailures = seriesInstanceEvents
                .ApplyAggregation((string?)null, (failedMasterId, tuple) => tuple.Item2 == null ? tuple.series.Id : null);
            var seriesInstanceEventsWithoutFailures = seriesInstanceEventsWithFailures
                .TakeWhile(tuple => tuple.Item2 != null) //// TODO if you can move this before the applyaggregation somehow, that would be ideal since we then wouldn't continue retrieve the first series instance if we already know we aren't returing anymore of them
                .Where(tuple => tuple.Item2?.Any() == true)
                .Select(tuple => new CalendarEvent() { Body = tuple.series.Body, Id = tuple.series.Id, Subject = tuple.series.Subject, Start = tuple.Item2?.First().Start });

            // because the second parameter of lastRequestedPageUrl is a fully realized value, we need to have actually enumerated the elements that we expect to return in the
            // first parameter; in this case, we have enumerated the elements because accessing seriesInstanceEventsWithFailures.Aggregation will enumerate enough events to
            // perform the aggregation; in a previous iteration of this method, the elements were enumerated with a .ToList() call
            return new ODataCollection<CalendarEvent>(
                seriesInstanceEventsWithoutFailures,
                seriesInstanceEventsWithFailures.Aggregation == null ?
                    seriesEventMasters.LastRequestedPageUrl :
                    $"/me/calendar/events/{seriesInstanceEventsWithFailures.Aggregation}");
        }

        private static QueryResult<CalendarEvent, (string, Exception)> Adapt(QueryResult<(CalendarEvent, QueryResult<CalendarEvent, (string, Exception)>), (string, Exception)> seriesInstanceEvents)
        {
            if (seriesInstanceEvents is QueryResult<(CalendarEvent, QueryResult<CalendarEvent, (string, Exception)>), (string, Exception)>.Final)
            {
                // there are no series masters
                return new QueryResult<CalendarEvent, (string, Exception)>.Final();
            }
            else if (seriesInstanceEvents is QueryResult<(CalendarEvent, QueryResult<CalendarEvent, (string, Exception)>), (string, Exception)>.Partial partial)
            {
                return new QueryResult<CalendarEvent, (string, Exception)>.Partial(partial.Error);
            }
            else if (seriesInstanceEvents is QueryResult<(CalendarEvent, QueryResult<CalendarEvent, (string, Exception)>), (string, Exception)>.Element element)
            {
                throw new Exception("TODO");
            }
            else
            {
                throw new Exception("TODO use visitor");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the requests
        /// </exception>
        private static async Task<QueryResult<CalendarEvent, (string, Exception)>> GetSeriesEventMasters(IGraphClient graphClient, int pageSize)
        {
            //// TODO make the calendar that's used configurable?
            var url = $"/me/calendar/events?" +
                $"$select=body,start,subject&" +
                $"$top={pageSize}&" +
                $"$orderBy=start/dateTime&" +
                "$filter=type eq 'seriesMaster' and isCancelled eq false";
            return await GetQueryResult<CalendarEvent>(graphClient, new Uri(url, UriKind.Relative).ToRelativeUri()).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="seriesMaster"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns>The first instance of the <paramref name="seriesMaster"/> event, or <see langword="null"/> if an error occurred while retrieveing the first instance</returns>
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the requests
        /// </exception>
        private static async Task<QueryResult<CalendarEvent, (string, Exception)>> GetFirstSeriesInstanceInRange(IGraphClient graphClient, CalendarEvent seriesMaster, DateTime startTime, DateTime endTime)
        {
            //// TODO this method would be much better as some sort of "TryGet" variant (though it does still have exceptions that it throws...), but being async, we can't have the needed out parameters
            var url = $"/me/calendar/events/{seriesMaster.Id}/instances?startDateTime={startTime}&endDateTime={endTime}&$top=1&$select=id,start,subject,body&$filter=isCancelled eq false";
            HttpResponseMessage? httpResponse = null;
            try
            {
                try
                {
                    httpResponse = await graphClient.GetAsync(new Uri(url, UriKind.Relative).ToRelativeUri()).ConfigureAwait(false);
                }
                catch (HttpRequestException e)
                {
                    return new QueryResult<CalendarEvent, (string, Exception)>.Partial((url, e));
                }

                try
                {
                    httpResponse.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException e)
                {
                    return new QueryResult<CalendarEvent, (string, Exception)>.Partial((url, e));
                }

                var httpResponseContent = await httpResponse.Content.ReadAsStringAsync().ConfigureAwait(false);
                ODataCollectionPage<CalendarEvent>.Builder? odataCollection;
                try
                {
                    odataCollection = JsonSerializer.Deserialize<ODataCollectionPage<CalendarEvent>.Builder>(httpResponseContent);
                }
                catch (JsonException e)
                {
                    return new QueryResult<CalendarEvent, (string, Exception)>.Partial((url, e));
                }

                if (odataCollection?.Value == null)
                {
                    return new QueryResult<CalendarEvent, (string, Exception)>.Partial((url, new Exception())); //// TODO the exception part isn't relevant here...
                }

                return odataCollection.Value.ToQueryResult<CalendarEvent, (string, Exception)>();
            }
            finally
            {
                httpResponse?.Dispose();
            }
        }

        private sealed class GraphCalendarEvent
        {
            [JsonPropertyName("id")]
            public string? Id { get; set; }

            [JsonPropertyName("subject")]
            public string? Subject { get; set; }

            [JsonPropertyName("start")]
            public TimeStructure? Start { get; set; }

            [JsonPropertyName("body")]
            public BodyStructure? Body { get; set; }

            public Either<CalendarEvent, GraphCalendarEvent> Build()
            {
                DateTimeOffset start;
                if (this.Id == null ||
                    this.Subject == null ||
                    this.Start == null ||
                    this.Start.DateTime == null ||
                    this.Start.TimeZone == null ||
                    !DateTimeOffset.TryParse(this.Start.DateTime, out start) ||
                    this.Body == null ||
                    this.Body.Content == null)
                {
                    return new Either<CalendarEvent, GraphCalendarEvent>.Right(this);
                }
                else
                {
                    return new Either<CalendarEvent, GraphCalendarEvent>.Left(
                        new CalendarEvent(this.Id, this.Id, this.Body.Content, start));
                }
            }
        }

        private sealed class BodyStructure
        {
            [JsonPropertyName("content")]
            public string? Content { get; set; }
        }

        private sealed class TimeStructure
        {
            [JsonPropertyName("dateTime")]
            public string? DateTime { get; set; }

            [JsonPropertyName("timeZone")]
            public string? TimeZone { get; set; }
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

        private static async Task<QueryResult<T, OdataError>> GetQueryResult<T>(IGraphClient graphClient, RelativeUri relativeUri)
        {
            ODataCollectionPage<T> page;
            try
            {
                page = await GetPage<T>(graphClient, relativeUri).ConfigureAwait(false);
            }
            catch (Exception e) //// TODO exception types
            {
                return new QueryResult<T, OdataError>.Partial(new OdataError(relativeUri.OriginalString, e));
            }

            return await GetQueryResult(graphClient, page).ConfigureAwait(false);
        }

        private static async Task<QueryResult<T, OdataError>> GetQueryResult<T>(IGraphClient graphClient, ODataCollectionPage<T> page)
        {
            var next = GetQueryResult<T>(graphClient, page.NextLink); //// TODO is there an issue with a stackoverflow or anything in this recursion?
            for (int i = page.Value.Count - 1; i >= 0; --i)
            {
                //// TODO why does implicit type conversion not work here?
                next = Task.FromResult<QueryResult<T, OdataError>>(new QueryResult<T, OdataError>.Element(page.Value[i], next));
            }

            return await next.ConfigureAwait(false);
        }

        private static async Task<QueryResult<T, OdataError>> GetQueryResult<T>(IGraphClient graphClient, string? nextLink)
        {
            if (nextLink == null)
            {
                return new QueryResult<T, OdataError>.Final();
            }

            AbsoluteUri nextLinkUri;
            try
            {
                nextLinkUri = new Uri(nextLink, UriKind.Absolute).ToAbsoluteUri();
            }
            catch (UriFormatException e)
            {
                return new QueryResult<T, OdataError>.Partial(new OdataError(nextLink, e));
            }

            ODataCollectionPage<T> page;
            try
            {
                page = await GetPage<T>(graphClient, nextLinkUri).ConfigureAwait(false);
            }
            catch (Exception e) //// TODO exception types
            {
                return new QueryResult<T, OdataError>.Partial(new OdataError(nextLinkUri.OriginalString, e));
            }

            return await GetQueryResult(graphClient, page).ConfigureAwait(false);
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
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the request
        /// </exception>
        /// <exception cref="GraphException">Thrown if graph produced an error while retrieving the page</exception>
        /// <exception cref="JsonException">Thrown if the response content was not a valid OData collection payload</exception>
        private static async Task<ODataCollectionPage<T>> GetPage<T>(IGraphClient graphClient, RelativeUri uri)
        {
            using (var httpResponse = await graphClient.GetAsync(uri).ConfigureAwait(false))
            {
                return await ReadPage<T>(httpResponse);
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
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the request
        /// </exception>
        /// <exception cref="GraphException">Thrown if graph produced an error while retrieving the page</exception>
        /// <exception cref="JsonException">Thrown if the response content was not a valid OData collection payload</exception>
        private static async Task<ODataCollectionPage<T>> GetPage<T>(IGraphClient graphClient, AbsoluteUri uri)
        {
            using (var httpResponse = await graphClient.GetAsync(uri).ConfigureAwait(false))
            {
                return await ReadPage<T>(httpResponse);
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
                //// TODO throw new GraphException(httpResponseContent, e);
                throw new Exception(httpResponseContent, e);
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
            private ODataCollectionPage(IReadOnlyList<T> value, string? nextLink)
            {
                this.Value = value;
                this.NextLink = nextLink;
            }

            public IReadOnlyList<T> Value { get; }

            /// <summary>
            /// Gets the URL of the next page in the collection, <see langword="null"/> if there are no more pages in the collection
            /// </summary>
            public string? NextLink { get; }

            public sealed class Builder
            {
                [JsonPropertyName("value")]
                public IReadOnlyList<T>? Value { get; set; }

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
