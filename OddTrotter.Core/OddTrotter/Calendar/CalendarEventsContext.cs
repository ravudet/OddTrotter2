////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    using System;
    using System.Linq.Expressions;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using OddTrotter.GraphClient;
    using OddTrotter.TodoList;

    public sealed class CalendarEventsContextPagingException : Exception
    {
        public CalendarEventsContextPagingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    public sealed class CalendarEventsContextTranslationException : Exception
    {
        public CalendarEventsContextTranslationException(string message)
            : base(message)
        {
        }

        public CalendarEventsContextTranslationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class CalendarEventsContext : IQueryContext<Either<CalendarEvent, CalendarEventsContextTranslationException>, CalendarEventsContextPagingException>, IWhereQueryContextMixin<CalendarEvent, CalendarEventsContextTranslationException, CalendarEventsContextPagingException, CalendarEventsContext>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<QueryResult<Either<CalendarEvent, CalendarEventsContextTranslationException>, CalendarEventsContextPagingException>> Evaluate()
        {
            var instanceEvents = await this.GetInstanceEvents().ConfigureAwait(false);
            //// TODO you are here
            var seriesEvents = await this.GetSeriesEvents().ConfigureAwait(false);
            //// TODO merge the sorted sequences instead of concat
            var allEvents = instanceEvents.Concat(seriesEvents);
            return allEvents;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphResponse"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphResponse"/> is <see langword="null"/></exception>
        private static
            QueryResult<Either<CalendarEvent, CalendarEventsContextTranslationException>, CalendarEventsContextPagingException>
            Adapt(
            QueryResult<Either<OddTrotter.Calendar.GraphCalendarEvent, GraphCalendarEventsContextTranslationException>, GraphPagingException> graphResponse)
        {
            if (graphResponse == null)
            {
                throw new ArgumentNullException(nameof(graphResponse));
            }

            return graphResponse
                .ErrorSelect(
                    graphPagingException => 
                        new CalendarEventsContextPagingException("An error occurred while paging through all of the calendar events.", graphPagingException))
                .Select(
                    graphCalendarEvent =>
                        graphCalendarEvent
                            .VisitSelect(
                                left => ToCalendarEvent(left),
                                right => 
                                    new CalendarEventsContextTranslationException(
                                        $"An error occurred while translating the OData response into a Graph calendar event",
                                        right))
                            .ShiftRight());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphClient"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        private async Task<QueryResult<Either<CalendarEvent, CalendarEventsContextTranslationException>, CalendarEventsContextPagingException>> GetInstanceEvents()
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
            var graphResponse = await this
                .graphCalendarEventsContext
                .Page(
                    graphQuery, 
                    nextLink =>  nextLink.StartsWith(this.graphCalendarEventsContext.ServiceRoot) ? this.graphCalendarEventsContext : throw new Exception("TODO you need a new exception type for this probably?")) //// TODO this contextgenerator stuff was because you didn't have serviceroot on the context interface, so you wanted the caller to pass it in; now that you have it in the interface, instead of a generator, you should probably just take in the "dictionary"; the reason this is coming up is because otherwise the `page` method needs to describe how the generator should return (or throw) in the even that a context cannot be found by the caller; you really don't want the generator to throw because that defeats the purpose of the queryresult stuff
                .ConfigureAwait(false);
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
        private async Task<QueryResult<Either<CalendarEvent, CalendarEventsContextTranslationException>, CalendarEventsContextPagingException>> GetSeriesEvents()
        {
            //// TODO you are here
            var seriesEventMasters =
                await this.GetSeriesEventMasters().ConfigureAwait(false);
            var mastersWithInstances = seriesEventMasters
                .Select(either => either.VisitSelect(
                    left => (left, GetFirstSeriesInstance(left).ConfigureAwait(false).GetAwaiter().GetResult()), //// TODO figure out how to make this async
                    right => right))
                .Select(eventPair => eventPair.ShiftRight())
                .Select(eventPair => eventPair.VisitSelect(
                    left => left.Item1,
                    right => right));
            return mastersWithInstances;
        }

        private async Task<Either<CalendarEvent, CalendarEventsContextTranslationException>> GetFirstSeriesInstance(
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
                //// TODO this response now has a "serviceroot" component; do you need to leverage it here?
                graphResponse = await this.graphCalendarEventsContext.Evaluate(graphRequest).ConfigureAwait(false); //// TODO do paging on the instances? it really shouldn't be necessary... //// TODO technically the serivce could return a bunch of empty pages with nextlinks...
            }
            catch
            {
                return Either.Left<CalendarEvent>().Right(new CalendarEventsContextTranslationException("TODO")); //// TODO
            }


            //// TODO do you want to do this check in the caller? someone up in the call stack probably wants to differentiate between mechanical errors vs the logical error of there being no instance events for a series
            if (graphResponse.Events.Count == 0)
            {
                return Either.Left<CalendarEvent>().Right(new CalendarEventsContextTranslationException("TODO")); //// TODO
            }

            return graphResponse
                .Events[0]
                .VisitSelect(
                    left => ToCalendarEvent(left),
                    right => new CalendarEventsContextTranslationException("TODO")) //// TODO
                .ShiftRight();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphCalendarEvent"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphCalendarEvent"/> is <see langword="null"/></exception>
        private static Either<CalendarEvent, CalendarEventsContextTranslationException> ToCalendarEvent(OddTrotter.Calendar.GraphCalendarEvent graphCalendarEvent)
        {
            if (graphCalendarEvent == null)
            {
                throw new ArgumentNullException(nameof(graphCalendarEvent));
            }

            DateTime start;
            try
            {
                //// TODO handle timezone...
                start = DateTime.Parse(graphCalendarEvent.Start.DateTime);
            }
            catch (FormatException formatException)
            {
                return Either
                    .Left<CalendarEvent>()
                    .Right(
                        new CalendarEventsContextTranslationException(
                            $"An error occurred while parsing the start time of the event: '{JsonSerializer.Serialize(graphCalendarEvent)}'.", // we are trusting that we can serialize without exception since we have a non-null instance of a highly structured type
                            formatException));
            }

            return Either
                .Right<CalendarEventsContextTranslationException>()
                .Left(
                    new CalendarEvent(
                        graphCalendarEvent.Id,
                        graphCalendarEvent.Subject,
                        graphCalendarEvent.Body.Content,
                        start,
                        graphCalendarEvent.IsCancelled));
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
        private async Task<QueryResult<Either<CalendarEvent, CalendarEventsContextTranslationException>, CalendarEventsContextPagingException>> GetSeriesEventMasters()
        {
            var url = 
                $"{this.calendarUriPath.Path}/events?" +
                $"$select=body,start,subject,isCancelled&" +
                $"$top={this.pageSize}&" + // the graph API does not implement `$top` correctly; it returns a `@nextLink` even if it gives you all `pageSize` elements that are requested; for this reason, we can use `$top` for page size here
                $"$orderBy=start/dateTime&" +
                "$filter=type eq 'seriesMaster'";

            if (this.isCancelled != null)
            {
                url += $"and isCancelled eq {this.isCancelled.Value.ToString().ToLower()}";
            }

            //// TODO you are here
            var graphRequest = new GraphQuery.GetEvents(new Uri(url, UriKind.Relative).ToRelativeUri());
            //// TODO how do you want to deal with the `contextGenerator` here? if the next page goes to a different service altogether, then your access token won't work anymore; however, nothing is exposed yet that will let you know if your current graph context has the correct service root; the `contextGenerator` *could* throw if a service root is returned that isn't the current one, but again, that information isn't exposed on the graph context interface; always using the current context is an option (you should leave a comment if this is what you do) because graph doesn't currently ever point you somewhere else, but doing this would mean if graph ever *did* start doing that, it would be harder to debug
            var graphResponse = await this.graphCalendarEventsContext.Page(graphRequest).ConfigureAwait(false);
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

                return new CalendarEventsContext(this.graphCalendarEventsContext, this.calendarUriPath, this.startTime, this.pageSize, this.firstInstanceInSeriesLookahead, now, this.isCancelled);
            }
            else if (object.ReferenceEquals(predicate, IsNotCancelled))
            {
                if (this.isCancelled != null)
                {
                    // the caller can only provide `IsNotCancelled` right now, so if `isCancelled` is already set, it won't be changing
                    return this;
                }

                return new CalendarEventsContext(this.graphCalendarEventsContext, this.calendarUriPath, this.startTime, this.pageSize, this.firstInstanceInSeriesLookahead, this.endTime, false);
            }

            throw new NotImplementedException("TODO");
        }

        public static Expression<Func<CalendarEvent, bool>> StartLessThanNow { get; } = calendarEvent => calendarEvent.Start < DateTime.UtcNow; //// TODO will "now" constantly change?

        public static Expression<Func<CalendarEvent, bool>> IsNotCancelled { get; } = calendarEvent => !calendarEvent.IsCancelled;
    }
}
