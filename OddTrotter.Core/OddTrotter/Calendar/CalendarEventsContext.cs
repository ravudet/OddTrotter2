////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    using System;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Threading.Tasks;
    using OddTrotter.GraphClient;

    public sealed class CalendarEventsContextPagingException : Exception
    {
        public CalendarEventsContextPagingException(string message)
            : base(message)
        {
        }
    }

    public sealed class CalendarEventsContextTranslationError
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class CalendarEventsContext : IQueryContext<Either<CalendarEvent, CalendarEventsContextTranslationError>, CalendarEventsContextPagingException>, IWhereQueryContextMixin<CalendarEvent, CalendarEventsContextTranslationError, CalendarEventsContextPagingException, CalendarEventsContext>
    {
        //// TODO can you use a more general context?
        private readonly IGraphCalendarEventsContext graphCalendarEventsContext;

        private readonly UriPath calendarUriPath;

        /// <summary>
        /// This is required because the `/instances` call on series event masters requires both a start time and an end time; we can continually look further in the future for `endTime`, but we have no way (as far as I can tell) to pick a `startTime` that guarantees we don't miss any instance events. So, we need the caller to tell us the start time in order to handle the series event instances.
        /// </summary>
        private readonly DateTime startTime;

        private readonly int pageSize;

        /// <summary>
        /// TODO you will want to handle this better in the future when you have full support for different query options. For now, `null` means that they've not filtered on `startTime < {something}`, and not `null` means that this value is `{something}`
        /// </summary>
        private readonly DateTime? endTime;

        /// <summary>
        /// TODO you will want to handle this better in the future when you have full support for different query options. For now, `null` means that they've not filtered on `isCancelled`, and not `null` means that this value is the value they are searching for
        /// </summary>
        private readonly bool? isCancelled;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarEventsContext"/> class
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="calendarUri"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="settings"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphClient"/> is <see langword="null"/></exception>
        public CalendarEventsContext(
            IGraphClient graphClient,
            UriPath calendarUriPath,
            DateTime startTime,
            CalendarEventContextSettings settings)
            : this(
                  //// TODO you are here
                  CreateGraphCalendarEventsContext(graphClient ?? throw new ArgumentNullException(nameof(graphClient))),
                  calendarUriPath,
                  startTime,
                  settings.PageSize,
                  null,
                  null)
        {   
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient"></param>
        /// <returns></returns>
        private static GraphCalendarEventsContext CreateGraphCalendarEventsContext(IGraphClient graphClient)
        {
            var odataClient = new GraphClientToOdataClient(graphClient);
            var odataCalendarEventsContext = new OdataCalendarEventsContext(odataClient);
            return new GraphCalendarEventsContext(odataCalendarEventsContext); //// TODO use constructor injection
        }

        private CalendarEventsContext(
            IGraphCalendarEventsContext graphCalendarEventsContext,
            UriPath calendarUriPath,
            DateTime startTime,
            int pageSize,
            DateTime? endTime,
            bool? isCancelled)
        {
            this.graphCalendarEventsContext = graphCalendarEventsContext;
            this.calendarUriPath = calendarUriPath;
            this.startTime = startTime;
            this.pageSize = pageSize;
            this.endTime = endTime;
            this.isCancelled = isCancelled;
        }

        private sealed class GraphClientToOdataClient : IOdataClient
        {
            private readonly IGraphClient graphClient;

            public GraphClientToOdataClient(IGraphClient graphClient)
            {
                this.graphClient = graphClient;
            }

            public async Task<HttpResponseMessage> GetAsync(RelativeUri relativeUri)
            {
                return await this.graphClient.GetAsync(relativeUri).ConfigureAwait(false);
            }
        }

        public Task<QueryResult<Either<CalendarEvent, CalendarEventsContextTranslationError>, CalendarEventsContextPagingException>> Evaluate()
        {
            //// TODO you should be able to cast QueryResult<Either<CalendarEvent, GraphCalendarEvent>, IOException> to QueryResult<Either<CalendarEvent, GraphCalendarEvent>, Exception>

            //// TODO you finished implementing one method in graphcalendareventscontext; you don't know if it works

            //// TODO finish implementing this class
            //// TODO implement query result using an abastract method
            //// TODO write tests for todolistservice that confirm the URLs
            //// TODO convert todolistservice to use this class
            //// TODO update this class to try using odataquerybuilder, odatarequestevaluator, etc; or maybe try adding the pending calendar events stuff first, and then update this class to make it easier to share code

            //// TODO use this.calendarUri
            return Task.FromResult(this.GetEvents());
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
        private QueryResult<Either<CalendarEvent, CalendarEventsContextTranslationError>, CalendarEventsContextPagingException> GetEvents()
        {
            var instanceEvents = this.GetInstanceEvents();
            var seriesEvents = this.GetSeriesEvents();
            //// TODO merge the sorted sequences instead of concat
            return instanceEvents.Concat(seriesEvents);
        }

        private static
            QueryResult<Either<CalendarEvent, CalendarEventsContextTranslationError>, CalendarEventsContextPagingException>
            Adapt(
            QueryResult<Either<OddTrotter.Calendar.GraphCalendarEvent, GraphCalendarEventsContextTranslationError>, GraphPagingException> graphResponse)
        {
            return graphResponse
                .Error(graphPageingException => new CalendarEventsContextPagingException("TODO presreve the exception"))
                .Select(graphCalendarEvent =>
                    graphCalendarEvent.VisitSelect(
                        left => ToCalendarEvent(left),
                        right => new CalendarEventsContextTranslationError()));
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
        private QueryResult<Either<CalendarEvent, CalendarEventsContextTranslationError>, CalendarEventsContextPagingException> GetInstanceEvents()
        {
            //// TODO make the calendar that's used configurable?
            var url =
                $"/me/calendar/events?" +
                $"$select=body,start,subject,isCancelled&" +
                $"$top={this.pageSize}&" + //// TODO does pagesize actually do anything with the queryresult model? if it does, it's because the graph api is not implementing odata correctly and you should document this //// TODO do this for all URLs
                $"$orderBy=start/dateTime&" +
                $"$filter=type eq 'singleInstance' and start/dateTime gt '{this.startTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.000000")}' and isCancelled eq false";
            var graphQuery = new GraphQuery.GetEvents(new Uri(url, UriKind.Relative).ToRelativeUri());
            var graphResponse = this.graphCalendarEventsContext.Page(graphQuery);
            return Adapt(graphResponse);
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
        private QueryResult<Either<CalendarEvent, CalendarEventsContextTranslationError>, CalendarEventsContextPagingException> GetSeriesEvents()
        {
            var seriesEventMasters =
                this.GetSeriesEventMasters();
            var mastersWithInstances = seriesEventMasters
                .Select(either => either.VisitSelect(
                    left => (left, GetFirstSeriesInstance(left)),
                    right => right))
                .Select(eventPair => eventPair.ShiftRight())
                .Select(eventPair => eventPair.VisitSelect(
                    left => left.Item1,
                    right => right));
            return mastersWithInstances;
        }

        private Either<CalendarEvent, CalendarEventsContextTranslationError> GetFirstSeriesInstance(
            CalendarEvent seriesMaster)
        {
            var url = $"/me/calendar/events/{seriesMaster.Id}/instances?startDateTime={this.startTime}&endDateTime={this.endTime}&$top=1&$select=id,start,subject,body,isCancelled&$filter=isCancelled eq false";
            var graphRequest = new GraphQuery.GetEvents(new Uri(url, UriKind.Relative).ToRelativeUri());

            GraphCalendarEventsResponse graphResponse;
            try
            {
                graphResponse = this.graphCalendarEventsContext.Evaluate(graphRequest); //// TODO do paging on the instances? it really shouldn't be necessary...
            }
            catch
            {
                return Either.Left<CalendarEvent>().Right(new CalendarEventsContextTranslationError()); //// TODO
            }


            //// TODO do you want to do this check in the caller? someone up in the call stack probably wants to differentiate between mechanical errors vs the logical error of there being no instance events for a series
            if (graphResponse.Events.Count == 0)
            {
                return Either.Left<CalendarEvent>().Right(new CalendarEventsContextTranslationError()); //// TODO
            }

            return graphResponse
                .Events[0]
                .VisitSelect(
                    left => ToCalendarEvent(left),
                    right => new CalendarEventsContextTranslationError()); //// TODO
        }

        private static CalendarEvent ToCalendarEvent(OddTrotter.Calendar.GraphCalendarEvent graphCalendarEvent)
        {
            return new CalendarEvent(
                graphCalendarEvent.Id,
                graphCalendarEvent.Subject,
                graphCalendarEvent.Body.Content,
                DateTime.Parse(graphCalendarEvent.Start.DateTime), //// TODO what about datetime parsing errors?
                graphCalendarEvent.IsCancelled);
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
        private QueryResult<Either<CalendarEvent, CalendarEventsContextTranslationError>, CalendarEventsContextPagingException> GetSeriesEventMasters()
        {
            //// TODO make the calendar that's used configurable?
            var url = $"/me/calendar/events?" +
                $"$select=body,start,subject,isCancelled&" +
                $"$top={this.pageSize}&" +
                $"$orderBy=start/dateTime&" +
                "$filter=type eq 'seriesMaster' and isCancelled eq false";
            var graphRequest = new GraphQuery.GetEvents(new Uri(url, UriKind.Relative).ToRelativeUri());
            var graphResponse = this.graphCalendarEventsContext.Page(graphRequest);
            return Adapt(graphResponse);
        }

        public CalendarEventsContext Where(Expression<Func<CalendarEvent, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
