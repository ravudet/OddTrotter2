namespace OddTrotter.Calendar
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection.Metadata;

    public sealed class CalendarContextCalendarEvent
    {
        public CalendarContextCalendarEvent(string id, BodyStructure body, TimeStructure start, TimeStructure end, string subject, ResponseStatusStructure responseStatus, string webLink, string type, bool isCancelled)
        {
            Id = id;// ?? throw new ArgumentNullException(nameof(id));
            Body = body;// ?? throw new ArgumentNullException(nameof(body));
            Start = start;// ?? throw new ArgumentNullException(nameof(start));
            End = end;// ?? throw new ArgumentNullException(nameof(end));
            Subject = subject;// ?? throw new ArgumentNullException(nameof(subject));
            ResponseStatus = responseStatus;// ?? throw new ArgumentNullException(nameof(responseStatus));
            WebLink = webLink;// ?? throw new ArgumentNullException(nameof(webLink));
            Type = type;// ?? throw new ArgumentNullException(nameof(type));
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
        public CalendarContext(
            GraphCalendarContext graphCalendarContext,
            DateTime startTime,
            DateTime endTime)
        {
            //// TODO we  should take a IODataInstanceContext, which should be a generic
            this.Events = new CalendarEventContext(graphCalendarContext.Events, startTime, endTime);
        }

        public IV2Queryable<CalendarContextCalendarEvent> Events { get; }

        private sealed class CalendarEventContext : IV2Queryable<CalendarContextCalendarEvent>, IWhereQueryable<CalendarContextCalendarEvent>, IOrderByQueryable<CalendarContextCalendarEvent>
        {
            private readonly IODataCollectionContext<GraphCalendarContextEvent> graphCalendarEventsContext;

            private readonly DateTime startTime;

            private readonly DateTime endTime;

            private readonly int pageSize = 50; //// TODO configure this

            private readonly Expression<Func<GraphCalendarContextEvent, bool>>? where;

            ////private readonly Expression<Func<GraphCalendarContextEvent, TKey2>>? orderBy;

            public CalendarEventContext(
                IODataCollectionContext<GraphCalendarContextEvent> graphCalendarEventsContext,
                DateTime startTime,
                DateTime endTime)
                : this(graphCalendarEventsContext, startTime, endTime, null)
            {
            }

            private CalendarEventContext(
                IODataCollectionContext<GraphCalendarContextEvent> graphCalendarEventsContext,
                DateTime startTime,
                DateTime endTime,
                Expression<Func<GraphCalendarContextEvent, bool>>? where)
            {
                this.graphCalendarEventsContext = graphCalendarEventsContext;
                this.startTime = startTime;
                this.endTime = endTime;
                this.where = where;
            }

            public IV2Queryable<CalendarContextCalendarEvent> OrderBy<TKey>(Expression<Func<CalendarContextCalendarEvent, TKey>> keySelector)
            {
                //// TODO should this do a "thenby"?
                var translated = new Visitor<TKey>().Visit(keySelector);

                var lambda = translated as Expression<Func<GraphCalendarContextEvent, TKey>>;

                //// TODO what about multiple orderby calls?
                return new CalendarEventContext(this.graphCalendarEventsContext.OrderBy(lambda!), this.startTime, this.endTime, this.where);
            }

            public IV2Queryable<CalendarContextCalendarEvent> Where(Expression<Func<CalendarContextCalendarEvent, bool>> predicate)
            {
                //// TODO this should check if subject (and others) is being checked and do those in memory; for series masters, it should be done before retrieving the next event in the series

                Expression<Func<GraphCalendarContextEvent, bool>> equivalentPredicate = calendarEvent => calendarEvent.IsCancelled == false;

                var translated = new Visitor<bool>().Visit(predicate);

                /*var lambda = Expression.Lambda<Func<GraphCalendarContextEvent, bool>>(
                    translated, 
                    predicate.Parameters.Select(parameterExpression => Expression.Parameter(typeof(GraphCalendarContextEvent), parameterExpression.Name)));*/
                var lambda = translated as Expression<Func<GraphCalendarContextEvent, bool>>;

                //// TODO whwat about mutiple where calls?
                return new CalendarEventContext(this.graphCalendarEventsContext, this.startTime, this.endTime, lambda);
            }

            private sealed class Visitor<T2> : ExpressionVisitor
            {
                protected override Expression VisitParameter(ParameterExpression node)
                {
                    return Expression.Parameter(typeof(GraphCalendarContextEvent), node.Name);
                }

                protected override Expression VisitLambda<T>(Expression<T> node)
                {
                    //// TODO should the parametesr be "visited" instead?
                    return Expression.Lambda<Func<GraphCalendarContextEvent, T2>>(
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
                        //// TODO the get member call is a bit hacky
                        //// TODO you may need to traverse and translate node.Expression yourself...
                        var name = node.Member.Name;
                        var declaringType = node.Member.DeclaringType;
                        Type translatedType;
                        if (declaringType == typeof(CalendarContextCalendarEvent))
                        {
                            translatedType = typeof(GraphCalendarContextEvent);
                        }
                        else if (declaringType == typeof(TimeStructure))
                        {
                            translatedType = typeof(TimeStructure);
                        }
                        else
                        {
                            //// TODO other translated types here
                            throw new Exception("TODO");
                        }

                        var members = translatedType.GetMember(name);

                        return Expression.MakeMemberAccess(
                            this.Visit(node.Expression),
                            members?.First()!);
                        ////return Expression.Constant(true, typeof(bool?));

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
                //// TODO who should be responsible for calling 'touniversaltime'
                var startTime = this.startTime.ToUniversalTime();
                var endTime = this.endTime.ToUniversalTime();

                //// TODO you need to check that lastpage url
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                var events = this.graphCalendarEventsContext
                    .Filter(calendarEvent => calendarEvent.Type == "singleInstance")
                    .Filter(calendarEvent => calendarEvent.Start.DateTime > startTime && calendarEvent.Start.DateTime < endTime)
                    .Top(this.pageSize)
                    .Select(calendarEvent => calendarEvent.Id)
                    .Select(calendarEvent => calendarEvent.Body)
                    .Select(calendarEvent => calendarEvent.Start)
                    .Select(calendarEvent => calendarEvent.End)
                    .Select(calendarEvent => calendarEvent.Subject)
                    .Select(calendarEvent => calendarEvent.ResponseStatus)
                    .Select(calendarEvent => calendarEvent.WebLink)
                    .Select(calendarEvent => calendarEvent.Type)
                    .Select(calendarEvent => calendarEvent.IsCancelled); //// TODO you need to handle that not all values were selected, or you need to always select the values in CalendarContextCalendarEvent; that might actually be pretty reasonable though
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                if (this.where != null)
                {
                    events = events.Filter(this.where);
                }

                return events
                    .GetValues().GetAwaiter().GetResult() //// TODO make this async
                    .Elements
                    .Select(ToCalendarContextCalendarEvent);
            }

            private IEnumerable<CalendarContextCalendarEvent> GetSeriesEvents()
            {
                //// TODO you need to check that lastpage url
                var seriesMasters = this.graphCalendarEventsContext
                    .Filter(calendarEvent => calendarEvent.Type == "seriesMaster")
                    .Top(this.pageSize)
                    .Select(calendarEvent => calendarEvent.Id)
                    .Select(calendarEvent => calendarEvent.Body)
                    .Select(calendarEvent => calendarEvent.Start)
                    .Select(calendarEvent => calendarEvent.End)
                    .Select(calendarEvent => calendarEvent.Subject)
                    .Select(calendarEvent => calendarEvent.ResponseStatus)
                    .Select(calendarEvent => calendarEvent.WebLink)
                    .Select(calendarEvent => calendarEvent.Type)
                    .Select(calendarEvent => calendarEvent.IsCancelled);

                if (this.where != null)
                {
                    seriesMasters = seriesMasters.Filter(this.where);
                }

                var seriesMastersValues = seriesMasters
                    .GetValues().GetAwaiter().GetResult(); //// TODO make this async

                var seriesInstanceEvents = seriesMastersValues
                    .Elements
                    .Select(seriesMaster => (seriesMaster, GetFirstSeriesInstanceInRange(seriesMaster)))
                    .Where(tuple => tuple.Item2 != null)
                    .Select(tuple =>
                    {
                        tuple.seriesMaster.Start = tuple.Item2?.Start;
                        var result = ToCalendarContextCalendarEvent(tuple.seriesMaster);
                        return result;
                    });

                return seriesInstanceEvents;
            }

            private GraphCalendarContextEvent? GetFirstSeriesInstanceInRange(GraphCalendarContextEvent graphCalendarContextEvent)
            {
                //// TODO error handling
                var graphEvents = graphCalendarContextEvent
                    .Instances(this.startTime, this.endTime)
                    .Top(1)
                    .Select(calendarEvent => calendarEvent.Id)
                    .Select(calendarEvent => calendarEvent.Body)
                    .Select(calendarEvent => calendarEvent.Start)
                    .Select(calendarEvent => calendarEvent.End)
                    .Select(calendarEvent => calendarEvent.Subject)
                    .Select(calendarEvent => calendarEvent.ResponseStatus)
                    .Select(calendarEvent => calendarEvent.WebLink)
                    .Select(calendarEvent => calendarEvent.Type)
                    .Select(calendarEvent => calendarEvent.IsCancelled);

                if (this.where != null)
                {
                    graphEvents = graphEvents.Filter(this.where);
                }

                return graphEvents
                    .GetValues().GetAwaiter().GetResult() //// TODO make this async
                    .Elements
                    .FirstOrDefault(); //// TODO throw instead?
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

            public Unit<TSource> Unit<TSource>()
            {
                throw new NotImplementedException();
            }
        }
    }
}
