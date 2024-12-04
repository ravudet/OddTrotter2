﻿namespace OddTrotter.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    public abstract class GraphQuery //// TODO graphrequest?
    {
        private GraphQuery()
        {
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(GraphQuery node, TContext context)
            {
                return node.Accept(this, context);
            }

            public abstract TResult Dispatch(GraphQuery.Page node, TContext context);

            internal abstract TResult Dispatch(GraphQuery.GetEvents node, TContext context);
        }

        public sealed class Page : GraphQuery
        {
            internal Page(RelativeUri relativeUri)
            {
                //// TODO make this private
                this.RelativeUri = relativeUri;
            }

            //// TODO you should really do this, and have a lookup of the iodatastructuredcontext based on the schema + authority
            //// internal UriScheme Schema { get; }

            //// TODO you should really do this, and have a lookup of the iodatastructuredcontext based on the schema + authority
            //// internal UriAuthority Authority { get; }

            internal RelativeUri RelativeUri { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Dispatch(this, context);
            }
        }

        internal sealed class GetEvents : GraphQuery
        {
            public GetEvents(RelativeUri relativeUri)
            {
                RelativeUri = relativeUri;
            }

            public RelativeUri RelativeUri { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Dispatch(this, context);
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
        private readonly EvaluateVisitor evaluateVisitor;

        public GraphCalendarEventsContext(IOdataStructuredContext odataContext)
        {
            this.evaluateVisitor = new EvaluateVisitor(odataContext);
        }

        public GraphCalendarEventsResponse Evaluate(GraphQuery graphQuery)
        {
            return this.evaluateVisitor.Visit(graphQuery, default);
        }

        /// <summary>
        /// TODO should you pass the iodatastructuredcontext as the visitor context, or should it be a field in the visitor? i feel like the visitor context is supposed to be used for this that are contextual *per request*, so probably that means a field
        /// </summary>
        private sealed class EvaluateVisitor : GraphQuery.Visitor<GraphCalendarEventsResponse, Void>
        {
            private readonly IOdataStructuredContext odataContext;

            private readonly GetEventsDispatchVisitor getEventsDispatchVisitor;

            public EvaluateVisitor(IOdataStructuredContext odataContext)
            {
                this.odataContext = odataContext;
                this.getEventsDispatchVisitor = GetEventsDispatchVisitor.Instance;
            }

            public override GraphCalendarEventsResponse Dispatch(GraphQuery.Page node, Void context)
            {
                return GetPage(node.RelativeUri).ConfigureAwait(false).GetAwaiter().GetResult(); //// TODO use async methods
            }

            internal override GraphCalendarEventsResponse Dispatch(GraphQuery.GetEvents node, Void context)
            {
                return GetPage(node.RelativeUri).ConfigureAwait(false).GetAwaiter().GetResult(); //// TODO use async methods
            }

            private async Task<GraphCalendarEventsResponse> GetPage(RelativeUri url)
            {
                var odataCollectionRequest = new OdataGetCollectionRequest(url);
                var odataCollectionResponse = await this.odataContext.GetCollection(odataCollectionRequest).ConfigureAwait(false);

                return odataCollectionResponse
                    .VisitSelect(
                        left => this.getEventsDispatchVisitor.Visit(left, default),
                        right => new Exception("TODO"))
                    .ThrowRight();
            }

            private sealed class GetEventsDispatchVisitor : OdataCollectionResponse.Visitor<GraphCalendarEventsResponse, Void>
            {
                private readonly OdataCollectionValueVisitor odataCollectionValueVisitor;

                private GetEventsDispatchVisitor()
                {
                    this.odataCollectionValueVisitor = OdataCollectionValueVisitor.Instance;
                }

                public static GetEventsDispatchVisitor Instance { get; } = new GetEventsDispatchVisitor();

                public override GraphCalendarEventsResponse Dispatch(OdataCollectionResponse.Values node, Void context)
                {
                    var graphCalendarEvents = node
                        .Value
                        .Select(odataCollectionValue => this.odataCollectionValueVisitor.Visit(odataCollectionValue, default))
                        .ToList();

                    GraphQuery.Page? nextPage = null;
                    if (node.NextLink != null)
                    {
                        var nextLink = new Uri(node.NextLink);
                        RelativeUri relativeUri;
                        if (nextLink.IsAbsoluteUri)
                        {
                            //// TODO this assumes that the servicehost for the nextlink is the same as the one used in the odatacontext; this is not guaranteed
                            relativeUri = nextLink.ToAbsoluteUri().GetRelativeUri();
                        }
                        else
                        {
                            relativeUri = nextLink.ToRelativeUri();
                        }

                        nextPage = new GraphQuery.Page(relativeUri);
                    }

                    return new GraphCalendarEventsResponse(graphCalendarEvents, nextPage);
                }

                private sealed class OdataCollectionValueVisitor : OdataCollectionValue.Visitor<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationError>, Void>
                {
                    private OdataCollectionValueVisitor()
                    {
                    }

                    public static OdataCollectionValueVisitor Instance { get; } = new OdataCollectionValueVisitor();

                    internal sealed override Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationError> Dispatch(OdataCollectionValue.Json node, Void context)
                    {
                        GraphCalendarEventBuilder? graphCalendarEvent;
                        try
                        {
                            graphCalendarEvent = JsonSerializer.Deserialize<GraphCalendarEventBuilder>(node.Node);
                        }
                        catch
                        {
                            return Either.Left<GraphCalendarEvent>().Right(new GraphCalendarEventsContextTranslationError()); //// TODO presrve exception
                        }

                        if (graphCalendarEvent == null)
                        {
                            return Either.Left<GraphCalendarEvent>().Right(new GraphCalendarEventsContextTranslationError());
                        }

                        return graphCalendarEvent.Build();
                    }
                }

                private sealed class GraphCalendarEventBuilder
                {
                    [JsonPropertyName("id")]
                    public string? Id { get; set; }

                    [JsonPropertyName("subject")]
                    public string? Subject { get; set; }

                    [JsonPropertyName("start")]
                    public TimeStructureBuilder? Start { get; set; }

                    [JsonPropertyName("body")]
                    public BodyStructureBuilder? Body { get; set; }

                    [JsonPropertyName("isCancelled")]
                    public bool? IsCancelled { get; set; }

                    public Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationError> Build()
                    {
                        if (this.Id == null || this.Subject == null || this.Start == null || this.Body == null || this.IsCancelled == null)
                        {
                            return Either.Left<GraphCalendarEvent>().Right(new GraphCalendarEventsContextTranslationError()); //// TODO
                        }

                        var start = this.Start.Build();
                        var body = this.Body.Build();

                        return 
                            start
                            .Zip(body)
                            .VisitSelect(
                                left => new GraphCalendarEvent(this.Id, this.Subject, left.Item1, left.Item2, this.IsCancelled.Value),
                                right => right);
                    }
                }

                public sealed class BodyStructureBuilder
                {
                    [JsonPropertyName("context")]
                    public string? Content { get; set; }

                    public Either<BodyStructure, GraphCalendarEventsContextTranslationError> Build()
                    {
                        if (this.Content == null)
                        {
                            return Either.Left<BodyStructure>().Right(new GraphCalendarEventsContextTranslationError()); //// TODO
                        }

                        return Either.Right<GraphCalendarEventsContextTranslationError>().Left(new BodyStructure(this.Content));
                    }
                }

                public sealed class TimeStructureBuilder
                {
                    [JsonPropertyName("dateTime")]
                    public string? DateTime { get; set; }

                    [JsonPropertyName("timeZone")]
                    public string? TimeZone { get; set; }

                    public Either<TimeStructure, GraphCalendarEventsContextTranslationError> Build()
                    {
                        if (this.DateTime == null || this.TimeZone == null)
                        {
                            return Either.Left<TimeStructure>().Right(new GraphCalendarEventsContextTranslationError()); //// TODO
                        }

                        return Either.Right<GraphCalendarEventsContextTranslationError>().Left(new TimeStructure(this.DateTime, this.TimeZone));
                    }
                }
            }
        }
    }

    public sealed class GraphCalendarEvent
    {
        public GraphCalendarEvent(string id, string subject, TimeStructure start, BodyStructure body, bool isCancelled)
        {
            this.Id = id;
            this.Subject = subject;
            this.Start = start;
            this.Body = body;
            this.IsCancelled = isCancelled;
        }

        public string Id { get; set; }

        public string Subject { get; set; }

        public TimeStructure Start { get; set; }

        public BodyStructure Body { get; set; }

        public bool IsCancelled { get; set; }
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
