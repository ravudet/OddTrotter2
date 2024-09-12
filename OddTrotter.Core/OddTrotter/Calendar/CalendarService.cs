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
