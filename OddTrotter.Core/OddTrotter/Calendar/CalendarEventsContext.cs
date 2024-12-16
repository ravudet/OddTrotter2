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
        private readonly IGraphCalendarEventsContext graphCalendarEventsContext;

        private readonly UriPath calendarUriPath;

        /// <summary>
        /// This is required because the `/instances` call on series event masters requires both a start time and an end time; we can continually look further in the future for `endTime`, but we have no way (as far as I can tell) to pick a `startTime` that guarantees we don't miss any instance events. So, we need the caller to tell us the start time in order to handle the series event instances.
        /// </summary>
        private readonly DateTime startTime;

        private readonly int pageSize;

        private readonly TimeSpan firstInstanceInSeriesLookahead;

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
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphCalendarEventsContext"/> or <paramref name="calendarUriPath"/> or <paramref name="settings"/> is <see langword="null"/></exception>
        public CalendarEventsContext(
            IGraphCalendarEventsContext graphCalendarEventsContext,
            UriPath calendarUriPath,
            DateTime startTime,
            CalendarEventsContextSettings settings)
            : this(
                  graphCalendarEventsContext ?? throw new ArgumentNullException(nameof(graphCalendarEventsContext)),
                  calendarUriPath ?? throw new ArgumentNullException(nameof(calendarUriPath)),
                  startTime,
                  settings?.PageSize ?? throw new ArgumentNullException(nameof(settings)), //// TODO i *really* don't like setting this precedence
                  settings?.FirstInstanceInSeriesLookahead ?? throw new ArgumentNullException(nameof(settings)),
                  null,
                  null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphCalendarEventsContext"></param>
        /// <param name="calendarUriPath"></param>
        /// <param name="startTime"></param>
        /// <param name="pageSize"></param>
        /// <param name="endTime"></param>
        /// <param name="isCancelled"></param>
        private CalendarEventsContext(
            IGraphCalendarEventsContext graphCalendarEventsContext,
            UriPath calendarUriPath,
            DateTime startTime,
            int pageSize,
            TimeSpan firstInstanceInSeriesLookahead,
            DateTime? endTime,
            bool? isCancelled)
        {
            this.graphCalendarEventsContext = graphCalendarEventsContext;
            this.calendarUriPath = calendarUriPath;
            this.startTime = startTime;
            this.pageSize = pageSize;
            this.firstInstanceInSeriesLookahead = firstInstanceInSeriesLookahead;
            this.endTime = endTime;
            this.isCancelled = isCancelled;
        }

        public Task<QueryResult<Either<CalendarEvent, CalendarEventsContextTranslationError>, CalendarEventsContextPagingException>> Evaluate()
        {
            //// TODO you are here
            var instanceEvents = this.GetInstanceEvents();
            var seriesEvents = this.GetSeriesEvents();
            //// TODO merge the sorted sequences instead of concat
            var allEvents = instanceEvents.Concat(seriesEvents);
            return Task.FromResult(allEvents);
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
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// 
        /// <exception cref="UnauthorizedAccessTokenException">
        /// Thrown if the access token configured on <paramref name="graphClient"/> is invalid or provides insufficient privileges for the requests
        /// </exception>
        private QueryResult<Either<CalendarEvent, CalendarEventsContextTranslationError>, CalendarEventsContextPagingException> GetInstanceEvents()
        {
            var url =
                $"{this.calendarUriPath.Path}/events?" +
                $"$select=body,start,subject,isCancelled&" +
                $"$top={this.pageSize}&" + // the graph API does not implement `$top` correctly; it returns a `@nextLink` even if it gives you all `pageSize` elements that are requested; for this reason, we can use `$top` for page size here
                $"$orderBy=start/dateTime&" +
                $"$filter=type eq 'singleInstance' and start/dateTime gt '{this.startTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.000000")}'";

            if (this.endTime != null)
            {
                url += $" and end/dateTime lt '{this.endTime.Value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.000000")}'";
            }

            if (this.isCancelled != null)
            {
                url += $" and isCancelled eq {this.isCancelled.Value.ToString().ToLower()}";
            }

            var graphQuery = new GraphQuery.GetEvents(new Uri(url, UriKind.Relative).ToRelativeUri());
            //// TODO you are here
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
            //// TODO make the calendar configurable
            var url =
                $"/me/calendar/events/{seriesMaster.Id}/instances?startDateTime={this.startTime}&";

            DateTime endTime;
            if (this.endTime != null)
            {
                endTime = this.endTime.Value; //// TODO will it be confusing that this *always* overrides `firstInstanceInSeriesLookahead`?
            }
            else
            {
                endTime = this.startTime + this.firstInstanceInSeriesLookahead;
            }

            url += $"endDateTime={this.endTime}&" +
                $"$top=1&$select=id,start,subject,body,isCancelled";

            if (this.isCancelled != null)
            {
                url += $"&$filter=isCancelled eq {this.isCancelled.Value.ToString().ToLower()}";
            }

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
            //// TODO make the calendar that's used configurable
            var url = $"/me/calendar/events?" +
                $"$select=body,start,subject,isCancelled&" +
                $"$top={this.pageSize}&" + // the graph API does not implement `$top` correctly; it returns a `@nextLink` even if it gives you all `pageSize` elements that are requested; for this reason, we can use `$top` for page size here
                $"$orderBy=start/dateTime&" +
                "$filter=type eq 'seriesMaster'";

            if (this.isCancelled != null)
            {
                url += $"and isCancelled eq {this.isCancelled.Value.ToString().ToLower()}";
            }

            var graphRequest = new GraphQuery.GetEvents(new Uri(url, UriKind.Relative).ToRelativeUri());
            var graphResponse = this.graphCalendarEventsContext.Page(graphRequest);
            return Adapt(graphResponse);
        }

        /// <inheritdoc/>
        public CalendarEventsContext Where(Expression<Func<CalendarEvent, bool>> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            if (object.ReferenceEquals(predicate, StartLessThanNow))
            {
                var now = DateTime.UtcNow;
                if (this.endTime != null && this.endTime < now)
                {
                    // we logically can see that this will always happen (they can only call set `endTime` to `DateTime.UtcNow`, so `now` will always been more in the future than `endTime`) 
                    return this;
                }

                return new CalendarEventsContext(this.graphCalendarEventsContext, this.calendarUriPath, this.startTime, this.pageSize, now, this.isCancelled);
            }
            else if (object.ReferenceEquals(predicate, IsNotCancelled))
            {
                if (this.isCancelled != null)
                {
                    // the caller can only provide `IsNotCancelled` right now, so if `isCancelled` is already set, it won't be changing
                    return this;
                }

                return new CalendarEventsContext(this.graphCalendarEventsContext, this.calendarUriPath, this.startTime, this.pageSize, this.endTime, false);
            }

            throw new NotImplementedException("TODO");
        }

        public static Expression<Func<CalendarEvent, bool>> StartLessThanNow { get; } = calendarEvent => calendarEvent.Start < DateTime.UtcNow; //// TODO will "now" constantly change?

        public static Expression<Func<CalendarEvent, bool>> IsNotCancelled { get; } = calendarEvent => !calendarEvent.IsCancelled;
    }
}
