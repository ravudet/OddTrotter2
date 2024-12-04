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
        private readonly IGraphClient graphClient;
        private readonly UriPath calendarUriPath;
        private readonly DateTime startTime;
        private readonly DateTime endTime;
        private readonly int pageSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarEventsContext"/> class
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="calendarUri"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="settings"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphClient"/> or <paramref name="calendarUriPath"/> is <see langword="null"/></exception>
        public CalendarEventsContext(
            IGraphClient graphClient,
            UriPath calendarUriPath,
            DateTime startTime,
            DateTime endTime,
            CalendarEventContextSettings settings)
        {
            if (graphClient == null)
            {
                throw new ArgumentNullException(nameof(graphClient));
            }

            if (calendarUriPath == null)
            {
                throw new ArgumentNullException(nameof(calendarUriPath));
            }

            this.graphClient = graphClient;
            this.calendarUriPath = calendarUriPath;
            this.startTime = startTime;
            //// TODO you are here
            //// TODO if endtime is datetime then you have to do a validation check; if it's a timespan then you can just compute the endtime
            //// TODO endtime and iscanceled should be done by the caller of the iquerycontext using queryable-like apis
            this.endTime = endTime;
            this.pageSize = settings.PageSize;

            var odataClient = new GraphClientToOdataClient(graphClient);
            var odataCalendarEventsContext = new OdataCalendarEventsContext(odataClient);
            this.graphCalendarEventsContext = new GraphCalendarEventsContext(odataCalendarEventsContext); //// TODO use constructor injection
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
            return Task.FromResult(this.GetEvents(this.graphClient, this.startTime, this.endTime, this.pageSize));
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
        private QueryResult<Either<CalendarEvent, CalendarEventsContextTranslationError>, CalendarEventsContextPagingException> GetEvents(IGraphClient graphClient, DateTime startTime, DateTime endTime, int pageSize)
        {
            var instanceEvents = this.GetInstanceEvents(startTime, endTime, pageSize);
            var seriesEvents = GetSeriesEvents(this.graphCalendarEventsContext, startTime, endTime, pageSize);
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
        private QueryResult<Either<CalendarEvent, CalendarEventsContextTranslationError>, CalendarEventsContextPagingException> GetInstanceEvents(DateTime startTime, DateTime endTime, int pageSize)
        {
            //// TODO make the calendar that's used configurable?
            var url =
                $"/me/calendar/events?" +
                $"$select=body,start,subject,isCancelled&" +
                $"$top={pageSize}&" +
                $"$orderBy=start/dateTime&" +
                $"$filter=type eq 'singleInstance' and start/dateTime gt '{startTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.000000")}' and isCancelled eq false";
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
        private static QueryResult<Either<CalendarEvent, CalendarEventsContextTranslationError>, CalendarEventsContextPagingException> GetSeriesEvents(IGraphCalendarEventsContext graphCalendarEventsContext, DateTime startTime, DateTime endTime, int pageSize)
        {
            var seriesEventMasters =
                GetSeriesEventMasters(graphCalendarEventsContext, pageSize);
            var mastersWithInstances = seriesEventMasters
                .Select(either => either.VisitSelect(
                    left => (left, GetFirstSeriesInstance(graphCalendarEventsContext, left, startTime, endTime)),
                    right => right))
                .Select(eventPair => eventPair.ShiftRight())
                .Select(eventPair => eventPair.VisitSelect(
                    left => left.Item1,
                    right => right));
            return mastersWithInstances;
        }

        private static Either<CalendarEvent, CalendarEventsContextTranslationError> GetFirstSeriesInstance(
            IGraphCalendarEventsContext graphCalendarEventsContext,
            CalendarEvent seriesMaster,
            DateTime startTime,
            DateTime endTime)
        {
            var url = $"/me/calendar/events/{seriesMaster.Id}/instances?startDateTime={startTime}&endDateTime={endTime}&$top=1&$select=id,start,subject,body,isCancelled&$filter=isCancelled eq false";
            var graphRequest = new GraphQuery.GetEvents(new Uri(url, UriKind.Relative).ToRelativeUri());

            GraphCalendarEventsResponse graphResponse;
            try
            {
                graphResponse = graphCalendarEventsContext.Evaluate(graphRequest); //// TODO do paging on the instances? it really shouldn't be necessary...
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
        private static QueryResult<Either<CalendarEvent, CalendarEventsContextTranslationError>, CalendarEventsContextPagingException> GetSeriesEventMasters(IGraphCalendarEventsContext graphCalendarEventsContext, int pageSize)
        {
            //// TODO make the calendar that's used configurable?
            var url = $"/me/calendar/events?" +
                $"$select=body,start,subject,isCancelled&" +
                $"$top={pageSize}&" +
                $"$orderBy=start/dateTime&" +
                "$filter=type eq 'seriesMaster' and isCancelled eq false";
            var graphRequest = new GraphQuery.GetEvents(new Uri(url, UriKind.Relative).ToRelativeUri());
            var graphResponse = graphCalendarEventsContext.Page(graphRequest);
            return Adapt(graphResponse);
        }

        public CalendarEventsContext Where(Expression<Func<CalendarEvent, bool>> predicate)
        {
            throw new NotImplementedException();
        }
    }
}
