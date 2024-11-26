namespace OddTrotter.Calendar
{
    using System;
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

        internal sealed class GetEvents : GraphQuery
        {
            public GetEvents(RelativeUri relativeUri)
            {
                RelativeUri = relativeUri;
            }

            public RelativeUri RelativeUri { get; }
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

    public sealed class GraphPagingException : Exception
    {
        public GraphPagingException(string message)
            : base(message)
        {
        }
    }

    public static class GraphCalendarEventsContextExtensions
    {
        private sealed class PageQueryResult : QueryResult<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationError>, GraphPagingException>.Element
        {
            private readonly GraphCalendarEventsResponse graphCalendarEventsResponse;
            private readonly int index;
            private readonly IGraphCalendarEventsContext graphCalendarEventsContext;

            public PageQueryResult(GraphCalendarEventsResponse graphCalendarEventsResponse, int index, IGraphCalendarEventsContext graphCalendarEventsContext)
                : base(graphCalendarEventsResponse.Events[index])
            {
                this.graphCalendarEventsResponse = graphCalendarEventsResponse;
                this.index = index;
                this.graphCalendarEventsContext = graphCalendarEventsContext;
            }

            public override QueryResult<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationError>, GraphPagingException> Next()
            {
                if (this.index + 1 < this.graphCalendarEventsResponse.Events.Count)
                {
                    return new PageQueryResult(this.graphCalendarEventsResponse, this.index + 1, this.graphCalendarEventsContext);
                }

                if (this.graphCalendarEventsResponse.NextPage == null)
                {
                    return new QueryResult<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationError>, GraphPagingException>.Final();
                }

                return PageImpl(this.graphCalendarEventsContext, this.graphCalendarEventsResponse.NextPage);
            }
        }

        public static QueryResult<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationError>, GraphPagingException> Page(
            this IGraphCalendarEventsContext graphCalendarEventsContext,
            GraphQuery graphQuery)
        {
            if (!(graphQuery is GraphQuery.GetEvents))
            {
                throw new Exception("TODO don't page in the middle of pagination?");
            }

            return PageImpl(graphCalendarEventsContext, graphQuery);
        }

        private static QueryResult<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationError>, GraphPagingException> PageImpl(
            this IGraphCalendarEventsContext graphCalendarEventsContext,
            GraphQuery pageQuery)
        {
            GraphCalendarEventsResponse response;
            try
            {
                response = graphCalendarEventsContext.Evaluate(pageQuery);
            }
            catch
            {
                return new QueryResult<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationError>, GraphPagingException>.Partial(new GraphPagingException("TODO preserve exception"));
            }

            if (response.Events.Count == 0)
            {
                if (response.NextPage == null)
                {
                    return new QueryResult<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationError>, GraphPagingException>.Final();
                }
                else
                {
                    return PageImpl(graphCalendarEventsContext, response.NextPage);
                }
            }

            return new PageQueryResult(response, 0, graphCalendarEventsContext);
        }
    }
}
