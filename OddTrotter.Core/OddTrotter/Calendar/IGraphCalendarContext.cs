namespace OddTrotter.Calendar
{
    using System.Collections.Generic;

    public abstract class GraphQuery
    {
        private GraphQuery()
        {
        }

        public sealed class Page : GraphQuery
        {
            private Page()
            {
                //// TODO
            }
        }
    }

    public interface IGraphCalendarEventsContext
    {
        GraphCalendarEventsResponse Evaluate(GraphQuery graphQuery);
    }

    public sealed class GraphCalendarEventsResponse
    {
        public GraphCalendarEventsResponse(
            IReadOnlyList<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationError>> events, 
            GraphQuery.Page? nextPage)
        {
            this.Events = events;
            this.NextPage = nextPage;
        }

        public IReadOnlyList<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationError>> Events { get; }

        public GraphQuery.Page? NextPage { get; }
    }

    public sealed class GraphCalendarEventsContextTranslationError
    {
    }

    public sealed class GraphCalendarEventsContext : IGraphCalendarEventsContext
    {
        private readonly IOdataStructuredContext odataContext;

        public GraphCalendarEventsContext(IOdataStructuredContext odataContext)
        {
            this.odataContext = odataContext;
        }

        public GraphCalendarEventsResponse Evaluate(GraphQuery graphQuery)
        {

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
