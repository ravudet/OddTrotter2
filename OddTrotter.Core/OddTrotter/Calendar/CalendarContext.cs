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
            private readonly IODataCollectionContext<GraphCalendarEvent> graphCalendarEventsContext;

            public CalendarEventContext(IODataCollectionContext<GraphCalendarEvent> graphCalendarEventsContext)
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
                //// TODO
                return Enumerable.Empty<CalendarContextCalendarEvent>();
            }

            private IEnumerable<CalendarContextCalendarEvent> GetSeriesEvents()
            {
                //// TODO
                return Enumerable.Empty<CalendarContextCalendarEvent>();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
