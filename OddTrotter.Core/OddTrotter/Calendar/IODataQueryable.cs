namespace OddTrotter.Calendar
{
    using System;
    using System.Collections.Generic;

    using OddTrotter.GraphClient;

    public sealed class GraphCalendarContext : IODataInstanceContext
    {
        private readonly IGraphClient graphClient;

        private readonly RelativeUri calendarUri;

        public GraphCalendarContext(IGraphClient graphClient, RelativeUri calendarUri)
        {
            this.graphClient = graphClient;
            this.calendarUri = calendarUri;

            this.Events = new CalendarEventCollectionContext(this.graphClient, this.calendarUri);
        }

        public IODataCollectionContext<GraphCalendarEvent> Events { get; }

        private sealed class CalendarEventCollectionContext : IODataCollectionContext<GraphCalendarEvent>
        {
            private readonly IGraphClient graphClient;

            private readonly RelativeUri calendarUri;

            private readonly string? filter;

            private readonly string? select;

            public ODataCollection<GraphCalendarEvent> Values
            {
                get
                {
                }
            }

            public CalendarEventCollectionContext(IGraphClient graphClient, RelativeUri calendarUri)
            {
                this.graphClient = graphClient;
                this.calendarUri = calendarUri;
            }

            private CalendarEventCollectionContext(IGraphClient graphClient, RelativeUri calendarUri, string filter, string select)
            {
                this.graphClient = graphClient;
                this.calendarUri = calendarUri;
                this.filter = filter;
                this.select = select;
            }

            public IODataCollectionContext<GraphCalendarEvent> Filter()
            {
                throw new NotImplementedException();
            }

            public IODataCollectionContext<GraphCalendarEvent> Select()
            {
                throw new NotImplementedException();
            }
        }
    }

    public interface IODataInstanceContext
    {
        //// TODO
    }

    public interface IODataCollectionContext<T>
    {
        //// TODO do you want this? IEnumerator<T> GetEnumerator();

        ODataCollection<T> Values { get; }

        IODataCollectionContext<T> Select();

        IODataCollectionContext<T> Filter();

        //// TODO T this[somekey?] { get; }
    }

    public sealed class ODataCollection<T>
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
