namespace OddTrotter.Calendar
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

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

        private sealed class CalendarEventContext : IV2Queryable<CalendarContextCalendarEvent>
        {
            private readonly IODataCollectionContext<GraphCalendarContextEvent> graphCalendarEventsContext;

            private readonly DateTime startTime = DateTime.UtcNow; //// TODO configure these fields

            private readonly DateTime endTime = DateTime.UtcNow;

            public CalendarEventContext(IODataCollectionContext<GraphCalendarContextEvent> graphCalendarEventsContext)
            {
                this.graphCalendarEventsContext = graphCalendarEventsContext;
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
