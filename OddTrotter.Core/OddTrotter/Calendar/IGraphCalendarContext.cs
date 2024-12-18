namespace OddTrotter.Calendar
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="visitor"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="visitor"/> is <see langword="null"/></exception>
        /// <exception cref="Exception">Throws any of the exceptions that the <see cref="AsyncVisitor{TResult, TContext}.Dispatch"/> overloads can throw</exception> //// TODO is this good?
        protected abstract Task<TResult> AcceptAsync<TResult, TContext>(AsyncVisitor<TResult, TContext> visitor, TContext context);

        public abstract class AsyncVisitor<TResult, TContext>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Throws any of the exceptions that the <see cref="Dispatch"/> overloads can throw</exception> //// TODO is this good?
            public async Task<TResult> Visit(GraphQuery node, TContext context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                return await node.AcceptAsync(this, context).ConfigureAwait(false);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            public abstract Task<TResult> Dispatch(GraphQuery.Page node, TContext context);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            internal abstract Task<TResult> Dispatch(GraphQuery.GetEvents node, TContext context);
        }

        public sealed class Page : GraphQuery
        {
            internal Page(RelativeUri relativeUri)
            {
                if (relativeUri == null)
                {
                    throw new ArgumentNullException(nameof(relativeUri));
                }

                //// TODO make this private
                this.RelativeUri = relativeUri;
            }

            //// TODO you should really do this, and have a lookup of the iodatastructuredcontext based on the schema + authority
            //// internal UriScheme Schema { get; }

            //// TODO you should really do this, and have a lookup of the iodatastructuredcontext based on the schema + authority
            //// internal UriAuthority Authority { get; }

            internal RelativeUri RelativeUri { get; }

            /// <inheritdoc/>
            protected sealed override async Task<TResult> AcceptAsync<TResult, TContext>(AsyncVisitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return await visitor.Dispatch(this, context).ConfigureAwait(false);
            }
        }

        internal sealed class GetEvents : GraphQuery
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="relativeUri"></param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="relativeUri"/> is <see langword="null"/></exception>
            public GetEvents(RelativeUri relativeUri)
            {
                if (relativeUri == null)
                {
                    throw new ArgumentNullException(nameof(relativeUri));
                }

                this.RelativeUri = relativeUri;
            }

            public RelativeUri RelativeUri { get; }

            /// <inheritdoc/>
            protected sealed override async Task<TResult> AcceptAsync<TResult, TContext>(AsyncVisitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return await visitor.Dispatch(this, context).ConfigureAwait(false);
            }
        }
    }

    public interface IGraphCalendarEventsContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphQuery"></param>
        /// <returns></returns>
        /// 
            //// TODO you are here
        Task<GraphCalendarEventsResponse> Evaluate(GraphQuery graphQuery);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphOdataContext"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphOdataContext"/> is <see langword="null"/></exception>
        public GraphCalendarEventsContext(IGraphOdataStructuredContext graphOdataContext)
        {
            if (graphOdataContext == null)
            {
                throw new ArgumentNullException(nameof(graphOdataContext));
            }

            this.evaluateVisitor = new EvaluateVisitor(graphOdataContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphQuery"></param>
        /// <returns></returns>
        public async Task<GraphCalendarEventsResponse> Evaluate(GraphQuery graphQuery)
        {
            //// TODO you are here
            return await this.evaluateVisitor.Visit(graphQuery, default).ConfigureAwait(false);
        }

        private sealed class EvaluateVisitor : GraphQuery.AsyncVisitor<GraphCalendarEventsResponse, Void>
        {
            private readonly IGraphOdataStructuredContext graphOdataContext;

            private readonly GetPageVisitor getPageVisitor;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="graphOdataContext"></param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphOdataContext"/> is <see langword="null"/></exception>
            public EvaluateVisitor(IGraphOdataStructuredContext graphOdataContext)
            {
                if (graphOdataContext == null)
                {
                    throw new ArgumentNullException(nameof(graphOdataContext));
                }

                this.graphOdataContext = graphOdataContext;
                this.getPageVisitor = GetPageVisitor.Instance;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            public sealed override async Task<GraphCalendarEventsResponse> Dispatch(GraphQuery.Page node, Void context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                //// TODO you are here
                return await GetPage(node.RelativeUri).ConfigureAwait(false);
            }

            internal sealed override async Task<GraphCalendarEventsResponse> Dispatch(GraphQuery.GetEvents node, Void context)
            {
                //// TODO you are here
                return await GetPage(node.RelativeUri).ConfigureAwait(false);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="url"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="url"/> is <see langword="null"/></exception>
            /// <exception cref="HttpRequestException">Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout, or the server responded with a payload that was not valid HTTP</exception> //// TODO this part about the invalid HTTP needs to be added to all of your xmldoc where you get an httprequestexception from httpclient
            /// <exception cref="OdataErrorDeserializationException">Thrown if an error occurred while deserializing the OData error response</exception>
            /// <exception cref="OdataSuccessDeserializationException">Thrown if an error occurred while deserializing the OData success response</exception>
            private async Task<GraphCalendarEventsResponse> GetPage(RelativeUri url)
            {
                if (url == null)
                {
                    throw new ArgumentNullException(nameof(url));
                }

                var odataCollectionRequest = new OdataGetCollectionRequest(url, Enumerable.Empty<HttpHeader>());
                var odataCollectionResponse = await this.graphOdataContext.GetCollection(odataCollectionRequest).ConfigureAwait(false);

                //// TODO you are here
                return odataCollectionResponse
                    .ResponseContent
                    .VisitSelect(
                        left => this.getPageVisitor.Visit(left, default),
                        right => new Exception("TODO"))
                    .ThrowRight();
            }

            private sealed class GetPageVisitor : OdataCollectionResponse.Visitor<GraphCalendarEventsResponse, Void>
            {
                private readonly OdataCollectionValueVisitor odataCollectionValueVisitor;

                /// <summary>
                /// 
                /// </summary>
                private GetPageVisitor()
                {
                    this.odataCollectionValueVisitor = OdataCollectionValueVisitor.Instance;
                }

                /// <summary>
                /// 
                /// </summary>
                public static GetPageVisitor Instance { get; } = new GetPageVisitor();

                /// <inheritdoc/>
                public override GraphCalendarEventsResponse Dispatch(OdataCollectionResponse.Values node, Void context)
                {
                    if (node == null)
                    {
                        throw new ArgumentNullException(nameof(node));
                    }

                    var graphCalendarEvents = node
                        .Value
                    //// TODO you are here
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
                    /// <summary>
                    /// 
                    /// </summary>
                    private OdataCollectionValueVisitor()
                    {
                    }

                    /// <summary>
                    /// 
                    /// </summary>
                    public static OdataCollectionValueVisitor Instance { get; } = new OdataCollectionValueVisitor();

                    /// <inheritdoc/>
                    internal sealed override Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationError> Dispatch(OdataCollectionValue.Json node, Void context)
                    {
                        if (node == null)
                        {
                            throw new ArgumentNullException(nameof(node));
                        }

                        //// TODO you are here
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

                //// TODO figure out async `queryresult`s
                return Page(this.graphCalendarEventsContext, this.graphCalendarEventsResponse.NextPage).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphCalendarEventsContext"></param>
        /// <param name="graphQuery">TODO this allows <paramref name="graphQuery"/> to be a paging query; do you want to protect against that for some reason?</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphCalendarEventsContext"/> or <paramref name="graphQuery"/> is <see langword="null"</exception>
        public static async Task<QueryResult<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationError>, GraphPagingException>> Page(
            this IGraphCalendarEventsContext graphCalendarEventsContext,
            GraphQuery graphQuery)
        {            
            if (graphCalendarEventsContext == null)
            {
                throw new ArgumentNullException(nameof(graphCalendarEventsContext));
            }

            if (graphQuery == null)
            {
                throw new ArgumentNullException(nameof(graphQuery));
            }

            GraphCalendarEventsResponse response;
            try
            {
                //// TODO you are here
                response = await graphCalendarEventsContext.Evaluate(graphQuery).ConfigureAwait(false);
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
                    return await Page(graphCalendarEventsContext, response.NextPage).ConfigureAwait(false);
                }
            }

            return new PageQueryResult(response, 0, graphCalendarEventsContext);
        }
    }
}
