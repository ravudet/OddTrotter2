namespace OddTrotter.Calendar
{
    using System.Collections.Generic;
    
    public interface IGraphCalendarEventContext
    {
        IEnumerable<GraphCalendarEvent> Evaluate(GraphQuery graphQuery);
    }

    public abstract class GraphQuery
    {
        private GraphQuery()
        {
        }
    }

    public sealed class GraphCalendarEventsContext : IGraphCalendarEventContext
    {
        private readonly IOdataStructuredContext odataContext;

        public GraphCalendarEventsContext(IOdataStructuredContext odataContext)
        {
            this.odataContext = odataContext;
        }

        public IEnumerable<GraphCalendarEvent> Evaluate(GraphQuery graphQuery)
        {
            throw new System.NotImplementedException();
        }
    }

    public sealed class GraphCalendarEvent
    {
        public GraphCalendarEvent(string id, string subject, TimeStructure start, BodyStructure body)
        {
            this.Id = id;
            this.Subject = subject;
            this.Start = start;
            this.Body = body;
        }

        public string Id { get; set; }

        public string Subject { get; set; }

        public TimeStructure Start { get; set; }

        public BodyStructure Body { get; set; }
    }

    public sealed class BodyStructure
    {
        public BodyStructure(string content)
        {
            this.Content = content;
        }

        public string Content { get; set; }
    }

    public sealed class TimeStructure
    {
        public TimeStructure(string dateTime, string timeZone)
        {
            this.DateTime = dateTime;
            this.TimeZone = timeZone;
        }

        public string DateTime { get; set; }

        public string TimeZone { get; set; }
    }
}
