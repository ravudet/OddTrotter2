namespace OddTrotter.Calendar
{
    using System;
    using System.Collections;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    public interface IGraphCalendarEventsContext
    {
        //// TODO i think you like having the "context" *and* "evaluator" types because the context is pretty specific to GET requests (is this true? what about a `$select` on a `POST` request?) //// TODO as i typed this i became less convinced, but we should try it anyway

        Task<QueryResult<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationException>, GraphPagingException>> Evaluate();

        IGraphCalendarEventsContext Filter(Expression<Func<GraphCalendarEvent, bool>> filter);

        IGraphCalendarEventsContext Top(int top);

        IGraphCalendarEventsContext OrderBy<TOrder>(Expression<Func<GraphCalendarEvent, TOrder>> orderBy);
    }

    public sealed class GraphCalendarEventsContext : IGraphCalendarEventsContext
    {
        private readonly IGraphCalendarEventsEvaluator evaluator;
        private readonly UriPath calendarRoot;

        private readonly string? filter;
        private readonly string? orderBy;

        public GraphCalendarEventsContext(IGraphCalendarEventsEvaluator evaluator, UriPath calendarRoot)
            : this(evaluator, calendarRoot, null, null)
        {
        }

        private GraphCalendarEventsContext(IGraphCalendarEventsEvaluator evaluator, UriPath calendarRoot, string? filter, string? orderBy)
        {
            this.evaluator = evaluator;
            this.calendarRoot = calendarRoot;
            this.filter = filter;
            this.orderBy = orderBy;
        }

        public async Task<QueryResult<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationException>, GraphPagingException>> Evaluate()
        {
            //// TODO should the context or the evaluator be the one that knows about `/events`?
            var uri = new Uri($"{this.calendarRoot.Path}/events", UriKind.Relative).ToRelativeUri();
            var query = new GraphQuery.GetEvents(uri);
            return await this.evaluator.Page(query).ConfigureAwait(false);
        }

        public IGraphCalendarEventsContext Filter(Expression<Func<GraphCalendarEvent, bool>> filter)
        {
            string filterExpression;
            if (filter == TypeEqualsSingleInstance)
            {
                filterExpression = "type eq 'singleInstance'";
            }
            else if (filter == StartTimeGreaterThanNow)
            {
                filterExpression = $"start/dateTime gt '{DateTime.UtcNow.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.000000")}'"; //// TODO does touniversal time mess up if you're already in utc?
            }
            else
            {
                throw new NotImplementedException("TODO");
            }

            if (this.filter == null)
            {
                return new GraphCalendarEventsContext(this.evaluator, this.calendarRoot, filterExpression, this.orderBy);
            }
            else
            {
                return new GraphCalendarEventsContext(this.evaluator, this.calendarRoot, this.filter + " and " + filterExpression, this.orderBy);
            }
        }

        public IGraphCalendarEventsContext OrderBy<TOrder>(Expression<Func<GraphCalendarEvent, TOrder>> orderBy)
        {
            string orderByExpression;
            if (orderBy is Expression<Func<GraphCalendarEvent, string>> asString && asString == StartTime)
            {
                orderByExpression = "start/dateTime";
            }
            else
            {
                throw new NotImplementedException("TODO");
            }

            if (this.orderBy == null)
            {
                return new GraphCalendarEventsContext(this.evaluator, this.calendarRoot, this.filter, orderByExpression);
            }
            else
            {
                return new GraphCalendarEventsContext(this.evaluator, this.calendarRoot, this.filter, this.orderBy + "," + orderByExpression);
            }
        }

        public IGraphCalendarEventsContext Top(int top)
        {
            throw new NotImplementedException();
        }

        internal static Expression<Func<GraphCalendarEvent, bool>> TypeEqualsSingleInstance { get; } = calendarEvent => true; //// TODO how should you handle the fact that `calendarEvent/type` won't get selected? it still needs to be a property on `graphcalendarevent` so that you can write this expression

        internal static Expression<Func<GraphCalendarEvent, bool>> StartTimeGreaterThanNow { get; } = calendarEvent => DateTime.Parse(calendarEvent.Start.DateTime) > DateTime.UtcNow;

        internal static Expression<Func<GraphCalendarEvent, string>> StartTime { get; } = calendarEvent => calendarEvent.Start.DateTime;
    }
}
