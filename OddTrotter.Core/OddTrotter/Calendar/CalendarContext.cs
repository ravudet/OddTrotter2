namespace OddTrotter.Calendar
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public sealed class CalendarContextCalendarEvent
    {
        public CalendarContextCalendarEvent(string id, BodyStructure body, TimeStructure start, TimeStructure end, string subject, ResponseStatusStructure responseStatus, string webLink, string type, bool isCancelled)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            Start = start ?? throw new ArgumentNullException(nameof(start));
            End = end ?? throw new ArgumentNullException(nameof(end));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            ResponseStatus = responseStatus ?? throw new ArgumentNullException(nameof(responseStatus));
            WebLink = webLink ?? throw new ArgumentNullException(nameof(webLink));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            IsCancelled = isCancelled;
        }

        public string Id { get; }

        public BodyStructure Body { get; }

        public TimeStructure Start { get; }

        public TimeStructure End { get; }

        public string Subject { get; }

        public ResponseStatusStructure ResponseStatus { get; }

        public string WebLink { get; }

        public string Type { get; }

        public bool IsCancelled { get; }
    }

    public sealed class CalendarContext
    {
        public CalendarContext(GraphCalendarContext graphCalendarContext)
        {
            //// TODO we  should take a IODataInstanceContext, which should be a generic
            this.Events = new CalendarEventContext(graphCalendarContext.Events);
        }

        public IV2Queryable<CalendarContextCalendarEvent> Events { get; }

        private sealed class CalendarEventContext : IV2Queryable<CalendarContextCalendarEvent>, IWhereQueryable<CalendarContextCalendarEvent>
        {
            private readonly IODataCollectionContext<GraphCalendarContextEvent> graphCalendarEventsContext;

            private readonly DateTime startTime = DateTime.UtcNow; //// TODO configure these fields

            private readonly DateTime endTime = DateTime.UtcNow;

            public CalendarEventContext(IODataCollectionContext<GraphCalendarContextEvent> graphCalendarEventsContext)
            {
                this.graphCalendarEventsContext = graphCalendarEventsContext;
            }

            public IV2Queryable<CalendarContextCalendarEvent> Where(Expression<Func<CalendarContextCalendarEvent, bool>> predicate)
            {
                Expression<Func<GraphCalendarContextEvent, bool>> equivalentPredicate = calendarEvent => calendarEvent.IsCancelled == false;

                var translated = new Visitor().Visit(predicate);

                /*var lambda = Expression.Lambda<Func<GraphCalendarContextEvent, bool>>(
                    translated, 
                    predicate.Parameters.Select(parameterExpression => Expression.Parameter(typeof(GraphCalendarContextEvent), parameterExpression.Name)));*/
                var lambda = translated as Expression<Func<GraphCalendarContextEvent, bool>>;

                return new CalendarEventContext(this.graphCalendarEventsContext.Filter(lambda!));
            }

            private sealed class Visitor : ExpressionVisitor
            {
                /*protected override Expression VisitParameter(ParameterExpression node)
                {
                    return Expression.Parameter(typeof(GraphCalendarContextEvent), node.Name);
                }*/

                protected override Expression VisitLambda<T>(Expression<T> node)
                {
                    //// TODO should the parametesr be "visited" instead?
                    return Expression.Lambda<Func<GraphCalendarContextEvent, bool>>(
                        this.Visit(node.Body),
                        node.Parameters.Select(parameterExpression => Expression.Parameter(typeof(GraphCalendarContextEvent), parameterExpression.Name)));
                }

                protected override Expression VisitBinary(BinaryExpression node)
                {
                    //// TODO this is just hyper-specific; you need to generalize
                    if (node.NodeType == ExpressionType.Equal)
                    {
                        //// TODO you're not even checking which side needs to be casted...
                        return
                            Expression.Equal(
                                this.Visit(node.Left),
                                this.Visit(node.Right));
                                /*Expression.Convert(
                                    Expression.Constant(false, typeof(bool)), typeof(bool?)));*/
                    }

                    return base.VisitBinary(node);
                }

                protected override Expression VisitConstant(ConstantExpression node)
                {
                    //// TODO make sure this is a constant that's part a binary operation where one of the sides is a memberaccess on the parameter
                    return Expression.Convert(node, typeof(bool?));

                    ////return base.VisitConstant(node);
                }
                
                protected override Expression VisitMember(MemberExpression node)
                {
                    if (IsParameter(node))
                    {
                        return Expression.Constant(true, typeof(bool?));

                        //// TODO the get member call is a bit hacky
                        //// TODO you may need to traverse and translate node.Expression yourself...
                        ////return Expression.MakeMemberAccess(this.Visit(node.Expression), typeof(GraphCalendarContextEvent).GetMember(node.Member.Name)[0]);
                    }

                    return base.VisitMember(node);
                }

                private static bool IsParameter(MemberExpression memberExpression)
                {
                    //// TODO you need to know that the member access is on the parameter and not on something else
                    return true;
                }
            }

            public IEnumerator<CalendarContextCalendarEvent> GetEnumerator()
            {
                /*return this.graphCalendarEventsContext
                    .Values
                    .Elements
                    .Select(graphCalendarEvent => new CalendarContextCalendarEvent(
                        graphCalendarEvent.Id!,
                        graphCalendarEvent.Body!,
                        graphCalendarEvent.Start!,
                        graphCalendarEvent.End!,
                        graphCalendarEvent.Subject!,
                        graphCalendarEvent.ResponseStatus!,
                        graphCalendarEvent.WebLink!,
                        graphCalendarEvent.Type!,
                        graphCalendarEvent.IsCancelled!.Value))
                    .GetEnumerator();*/
                return GetEvents().GetEnumerator();
            }

            private IEnumerable<CalendarContextCalendarEvent> GetEvents()
            {
                return this.GetInstanceEvents().Concat(this.GetSeriesEvents());
            }

            private IEnumerable<CalendarContextCalendarEvent> GetInstanceEvents()
            {
                //// TODO you need to check that lastpage url
                return this.graphCalendarEventsContext
                    .Filter(calendarEvent => calendarEvent.Type == "singleInstance")
                    .Values
                    .Elements
                    .Select(ToCalendarContextCalendarEvent);
            }

            private IEnumerable<CalendarContextCalendarEvent> GetSeriesEvents()
            {
                //// TODO you need to check that lastpage url
                var seriesMasters = this.graphCalendarEventsContext.Filter(calendarEvent => calendarEvent.Type == "seriesMaster").Values;

                var seriesInstanceEvents = seriesMasters
                    .Elements
                    .Select(seriesMaster => (seriesMaster, GetFirstSeriesInstanceInRange(seriesMaster)))
                    .Select(tuple =>
                    {
                        tuple.seriesMaster.Start = tuple.Item2.Start;
                        var result = ToCalendarContextCalendarEvent(tuple.seriesMaster);
                        return result;
                    });

                return seriesInstanceEvents;
            }

            private GraphCalendarContextEvent GetFirstSeriesInstanceInRange(GraphCalendarContextEvent graphCalendarContextEvent)
            {
                //// TODO error handling
                var graphEvents = graphCalendarContextEvent
                    .Instances(this.startTime, this.endTime)
                    .Top(1)
                    .Select(calendarEvent => calendarEvent.Id)
                    .Select(calendarEvent => calendarEvent.Start)
                    .Select(calendarEvent => calendarEvent.Subject)
                    .Select(calendarEvent => calendarEvent.Body)
                    .Select(calendarEvent => calendarEvent.ResponseStatus)
                    .Select(calendarEvent => calendarEvent.WebLink)
                    .Filter(calendarEvent => calendarEvent.IsCancelled == false);
                    
                return graphEvents
                    .Values
                    .Elements
                    .First();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            private static CalendarContextCalendarEvent ToCalendarContextCalendarEvent(GraphCalendarContextEvent graphCalendarEvent)
            {
                return new CalendarContextCalendarEvent(
                    graphCalendarEvent.Id!,
                    graphCalendarEvent.Body!,
                    graphCalendarEvent.Start!,
                    graphCalendarEvent.End!,
                    graphCalendarEvent.Subject!,
                    graphCalendarEvent.ResponseStatus!,
                    graphCalendarEvent.WebLink!,
                    graphCalendarEvent.Type!,
                    graphCalendarEvent.IsCancelled!.Value);
            }
        }
    }
}
