namespace OddTrotter.Calendar
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

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
            internal Page(OdataNextLink nextLink)
            {
                if (nextLink == null)
                {
                    throw new ArgumentNullException(nameof(nextLink));
                }

                this.NextLink = nextLink;
            }

            internal OdataNextLink NextLink { get; }

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

    internal sealed class ServiceRoot
    {
        public ServiceRoot(Inners.Scheme scheme, Inners.Host host, uint? port, IEnumerable<Inners.Segment> segments)
        {
            Scheme = scheme;
            Host = host;
            Port = port; //// TODO make this not nullable
            Segments = segments;
        }

        public Inners.Scheme Scheme { get; }
        public Inners.Host Host { get; }
        public uint? Port { get; }
        public IEnumerable<Inners.Segment> Segments { get; }

        public static class Inners
        {
            public abstract class Scheme
            {
                private Scheme()
                {
                }

                public sealed class Https : Scheme
                {
                    private Https()
                    {
                    }

                    public static Https Instance { get; } = new Https();
                }

                public sealed class Http : Scheme
                {
                    private Http()
                    {
                    }

                    public static Http Instance { get; } = new Http();
                }
            }

            public sealed class Host
            {
                public Host(string value)
                {
                    if (value.Contains("/") || value.Contains(":") || value.Contains("?") || value.Contains("#"))
                    {
                        throw new ArgumentException("TODO");
                    }

                    try
                    {
                        new Uri(value);
                    }
                    catch
                    {
                        throw;
                    }

                    Value = value;
                }

                public string Value { get; }
            }

            public sealed class Segment
            {
                public Segment(string value)
                {
                    if (value.Contains("/") || value.Contains("?") || value.Contains("#"))
                    {
                        throw new ArgumentException("TODO");
                    }

                    try
                    {
                        new Uri(value);
                    }
                    catch
                    {
                        throw;
                    }

                    Value = value;
                }

                public string Value { get; }
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
            IReadOnlyList<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationException>> events,
            GraphQuery.Page? nextPage)
        {
            this.Events = events;
            this.NextPage = nextPage;
        }

        public IReadOnlyList<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationException>> Events { get; }

        public GraphQuery.Page? NextPage { get; }
    }

    public sealed class GraphCalendarEventsContextTranslationException : Exception
    {
        public GraphCalendarEventsContextTranslationException(string message, string rawEventContents)
            : base(message)
        {
            this.RawEventContents = rawEventContents; ///// tODO make sure you are doing `this.` everywhere
        }

        public GraphCalendarEventsContextTranslationException(string message, Exception innerException, string rawEventContents)
            : base(message, innerException)
        {
            this.RawEventContents = rawEventContents;
        }

        public string RawEventContents { get; }
    }

    public sealed class InvalidNextLinkException : Exception
    {
        public InvalidNextLinkException(string message, Exception innerException, string rawNextLink)
            : base(message, innerException)
        {
            this.rawNextLink = rawNextLink;
        }

        public string rawNextLink { get; }
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

                return odataCollectionResponse
                    .ResponseContent
                    .VisitSelect(
                //// TODO you are here
                //// TODO you skipped documenting the getcolleciton call above
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
                /// <exception cref="InvalidNextLinkException">Thrown if the response contains a "@odata.nextLink" that is not a valid URI</exception>
                public override GraphCalendarEventsResponse Dispatch(OdataCollectionResponse.Values node, Void context)
                {
                    if (node == null)
                    {
                        throw new ArgumentNullException(nameof(node));
                    }

                    var graphCalendarEvents = node
                        .Value
                        .Select(
                            odataCollectionValue => this.odataCollectionValueVisitor
                                .Visit(
                                    odataCollectionValue,
                                    default))
                        .ToList(); //// TODO do you want this to be lazy?

                    GraphQuery.Page? nextPage = null;
                    if (node.NextLink != null)
                    {
                        Uri nextLink;
                        try
                        {
                            nextLink = new Uri(node.NextLink);
                        }
                        catch (UriFormatException uriFormatException)
                        {
                            throw new InvalidNextLinkException("Graph responded with a '@odata.nextLink' that is not a valid URI.", uriFormatException, node.NextLink);
                        }

                        ServiceRoot? serviceRoot = null;
                        RelativeUri relativeUri;
                        if (nextLink.IsAbsoluteUri)
                        {
                            //// TODO you are here
                            (serviceRoot, relativeUri) = Parse(nextLink.ToAbsoluteUri());
                        }
                        else
                        {
                            relativeUri = nextLink.ToRelativeUri();
                        }

                        nextPage = new GraphQuery.Page(serviceRoot, relativeUri);
                    }

                    return new GraphCalendarEventsResponse(graphCalendarEvents, nextPage);
                }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="absoluteUri"></param>
                /// <returns></returns>
                /// <exception cref="ArgumentNullException">Thrown if <paramref name="absoluteUri"/> is <see langword="null"/></exception>
                private static (ServiceRoot ServiceRoot, RelativeUri RelativeUri) Parse(AbsoluteUri absoluteUri)
                {
                    if (absoluteUri == null)
                    {
                        throw new ArgumentNullException(nameof(absoluteUri));
                    }

                    //// TODO you are here
                    //// TODO you are currently adding this as odatanextlink in the iodatastructuredcontext
                    //// TODO shouldn't this be in the odata layer somewhere?
                    //// TODO you should probably do a proper parsing instead of all of this string manipulation
                    var schemeDelimiter = "://";
                    var schemeIndex = absoluteUri.OriginalString.IndexOf(schemeDelimiter, 0);
                    if (schemeIndex < 0)
                    {
                        throw new Exception("TODO");
                    }

                    var providedScheme = Substring2(absoluteUri.OriginalString, 0, schemeIndex);
                    ServiceRoot.Inners.Scheme scheme;
                    if (string.Equals(providedScheme, "https", StringComparison.OrdinalIgnoreCase))
                    {
                        scheme = ServiceRoot.Inners.Scheme.Https.Instance;
                    }
                    else if (string.Equals(providedScheme, "http", StringComparison.OrdinalIgnoreCase))
                    {
                        scheme = ServiceRoot.Inners.Scheme.Http.Instance;
                    }
                    else
                    {
                        throw new Exception("TODO");
                    }

                    var hostDelimiter = "/";
                    var hostIndex = absoluteUri.OriginalString.IndexOf(hostDelimiter, schemeIndex + 1);
                    if (hostIndex < 0)
                    {
                        throw new Exception("TODO");
                    }

                    var fullHost = Substring2(absoluteUri.OriginalString, schemeIndex + schemeDelimiter.Length, hostIndex);
                    var portDelimiter = ":";
                    var portIndex = fullHost.IndexOf(portDelimiter, 0);
                    uint? port;
                    ServiceRoot.Inners.Host host;
                    if (portIndex < 0)
                    {
                        host = new ServiceRoot.Inners.Host(fullHost);
                        port = null;
                    }
                    else
                    {
                        var providedHost = Substring2(fullHost, 0, portIndex);
                        host = new ServiceRoot.Inners.Host(providedHost);

                        var providedPort = Substring2(fullHost, portIndex + portDelimiter.Length, fullHost.Length);
                        try
                        {
                            port = uint.Parse(providedPort);
                        }
                        catch
                        {
                            throw;
                        }
                    }

                    //// TODO we are assuming that there are no segments in the service root; this is not a legitimate assumption overall, but it will work for graph requests

                    var providedRelativeUri = Substring2(absoluteUri.OriginalString, hostIndex + 1, absoluteUri.OriginalString.Length);
                    return
                        (
                            new ServiceRoot(
                                scheme,
                                host,
                                port,
                                Enumerable.Empty<ServiceRoot.Inners.Segment>()),
                            new Uri(providedRelativeUri, UriKind.Relative).ToRelativeUri()
                        );
                }

                private static string Substring2(string value, int inclusiveStartIndex, int exclusiveEndIndex)
                {
                    return value.Substring(inclusiveStartIndex, exclusiveEndIndex - inclusiveStartIndex);
                }

                private sealed class OdataCollectionValueVisitor : OdataCollectionValue.Visitor<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationException>, Void>
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
                    internal sealed override Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationException> Dispatch(OdataCollectionValue.Json node, Void context)
                    {
                        if (node == null)
                        {
                            throw new ArgumentNullException(nameof(node));
                        }

                        GraphCalendarEventBuilder? graphCalendarEvent;
                        try
                        {
                            graphCalendarEvent = JsonSerializer.Deserialize<GraphCalendarEventBuilder>(node.Node);
                        }
                        catch (JsonException jsonException) //// TODO always write out the exception name
                        {
                            return Either
                                .Left<GraphCalendarEvent>()
                                .Right(new GraphCalendarEventsContextTranslationException(
                                    "An error occurred while translating the OData collection element into a Graph calendar event",
                                    jsonException,
                                    node.Node.ToString()));
                        }

                        if (graphCalendarEvent == null)
                        {
                            // OData allows null element in collections, but we know that graph is not supposed to return null events
                            return Either
                                .Left<GraphCalendarEvent>()
                                .Right(new GraphCalendarEventsContextTranslationException(
                                    "An error occurred while translating the OData collection element into a Graph calendar event",
                                    node.Node.ToString()));
                        }

                        return graphCalendarEvent.Build(node.Node.ToString());
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

                    /// <summary>
                    /// 
                    /// </summary>
                    /// <param name="rawEventContents"></param>
                    /// <returns></returns>
                    public Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationException> Build(string rawEventContents)
                    {
                        var invalidities = new List<string>();

                        if (this.Id == null)
                        {
                            invalidities.Add($"'{nameof(Id)}' cannot be null");
                        }

                        if (this.Subject == null)
                        {
                            invalidities.Add($"'{nameof(Subject)}' cannot be null");
                        }

                        if (this.Start == null)
                        {
                            invalidities.Add($"'{nameof(Start)}' cannot be null");
                        }

                        if (this.Body == null)
                        {
                            invalidities.Add($"'{nameof(Body)}' cannot be null");
                        }

                        if (this.IsCancelled == null)
                        {
                            invalidities.Add($"'{nameof(IsCancelled)}' cannot be null");
                        }

                        if (invalidities.Count > 0)
                        { 
                            return Either
                                .Left<GraphCalendarEvent>()
                                .Right(new GraphCalendarEventsContextTranslationException(
                                    $"An error occurred while translating the OData collection element into a Graph calendar event: {string.Join(", ", invalidities)}",
                                    rawEventContents));
                        }

                        //// TODO see if you can avoid the bangs in the rest of the method
                        var start = this.Start!.Build(rawEventContents);
                        var body = this.Body!.Build(rawEventContents);

                        return
                            start
                            .Zip(
                                body,
                                (startError, bodyError) => new GraphCalendarEventsContextTranslationException(
                                    $"{startError.Message}. {bodyError.Message}.", //// TODO the period here is why every exception message should use proper punctuation
                                    rawEventContents))
                            .VisitSelect(
                                left => new GraphCalendarEvent(
                                    this.Id!,
                                    this.Subject!,
                                    left.Item1,
                                    left.Item2,
                                    this.IsCancelled!.Value),
                                right => right);
                    }
                }

                public sealed class BodyStructureBuilder
                {
                    [JsonPropertyName("context")]
                    public string? Content { get; set; }

                    /// <summary>
                    /// 
                    /// </summary>
                    /// <returns></returns>
                    public Either<BodyStructure, GraphCalendarEventsContextTranslationException> Build(string rawEventContents)
                    {
                        var invalidities = new List<string>();

                        if (this.Content == null)
                        {
                            invalidities.Add($"'{nameof(Content)}' cannot be null");
                        }

                        if (invalidities.Count > 0)
                        {
                            return Either
                                .Left<BodyStructure>()
                                .Right(new GraphCalendarEventsContextTranslationException(
                                    $"An error occurred while translating the OData collection element into the '{nameof(GraphCalendarEventBuilder.Body)}' portion of a Graph calendar event: {string.Join(", ", invalidities)}",
                                    rawEventContents));
                        }

                        return Either
                            .Right<GraphCalendarEventsContextTranslationException>()
                            .Left(new BodyStructure(
                                this.Content!)); //// TODO see if you can avoid the bang
                    }
                }

                public sealed class TimeStructureBuilder
                {
                    [JsonPropertyName("dateTime")]
                    public string? DateTime { get; set; }

                    [JsonPropertyName("timeZone")]
                    public string? TimeZone { get; set; }

                    /// <summary>
                    /// 
                    /// </summary>
                    /// <returns></returns>
                    public Either<TimeStructure, GraphCalendarEventsContextTranslationException> Build(string rawEventContents)
                    {
                        var invalidities = new List<string>();

                        if (this.DateTime == null)
                        {
                            invalidities.Add($"'{nameof(DateTime)}' cannot be null");
                        }

                        if (this.TimeZone == null)
                        {
                            invalidities.Add($"'{nameof(TimeZone)}' cannot be null");
                        }

                        if (invalidities.Count > 0)
                        {
                            return Either
                                .Left<TimeStructure>()
                                .Right(new GraphCalendarEventsContextTranslationException(
                                    $"An error occurred while translating the OData collection element into the '{nameof(GraphCalendarEventBuilder.Start)}' portion of a Graph calendar event: {string.Join(", ", invalidities)}",
                                    rawEventContents));
                        }

                        //// TODO see if you can avoid the bangs in the rest of this methods
                        return Either
                            .Right<GraphCalendarEventsContextTranslationException>()
                            .Left(new TimeStructure(
                                this.DateTime!, 
                                this.TimeZone!));
                    }
                }
            }
        }
    }

    public sealed class GraphCalendarEvent
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="subject"></param>
        /// <param name="start"></param>
        /// <param name="body"></param>
        /// <param name="isCancelled"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="id"/> or <paramref name="subject"/> or <paramref name="start"/> or <paramref name="body"/> is <see langword="null"/></exception>
        public GraphCalendarEvent(string id, string subject, TimeStructure start, BodyStructure body, bool isCancelled)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (subject == null)
            {
                throw new ArgumentNullException(nameof(subject));
            }

            if (start == null)
            {
                throw new ArgumentNullException(nameof(start));
            }

            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="content"/> is <see langword="null"/></exception>
        public BodyStructure(string content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            this.Content = content;
        }

        public string Content { get; set; }
    }

    public sealed class TimeStructure
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="timeZone"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="dateTime"/> or <paramref name="timeZone"/> is <see langword="null"/></exception>
        public TimeStructure(string dateTime, string timeZone)
        {
            if (dateTime == null)
            {
                throw new ArgumentNullException(nameof(dateTime));
            }

            if (timeZone == null)
            {
                throw new ArgumentNullException(nameof(timeZone));
            }

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
        private sealed class PageQueryResult : QueryResult<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationException>, GraphPagingException>.Element
        {
            private readonly GraphCalendarEventsResponse graphCalendarEventsResponse;
            private readonly int index;
            private readonly IGraphCalendarEventsContext graphCalendarEventsContext;
            private readonly Func<GraphQuery.Page, IGraphCalendarEventsContext> contextGenerator;

            public PageQueryResult(GraphCalendarEventsResponse graphCalendarEventsResponse, int index, IGraphCalendarEventsContext graphCalendarEventsContext, Func<GraphQuery.Page, IGraphCalendarEventsContext> contextGenerator)
                : base(graphCalendarEventsResponse.Events[index])
            {
                this.graphCalendarEventsResponse = graphCalendarEventsResponse;
                this.index = index;
                this.graphCalendarEventsContext = graphCalendarEventsContext;
                this.contextGenerator = contextGenerator;
            }

            public override QueryResult<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationException>, GraphPagingException> Next()
            {
                if (this.index + 1 < this.graphCalendarEventsResponse.Events.Count)
                {
                    return new PageQueryResult(this.graphCalendarEventsResponse, this.index + 1, this.graphCalendarEventsContext, this.contextGenerator);
                }

                if (this.graphCalendarEventsResponse.NextPage == null)
                {
                    return new QueryResult<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationException>, GraphPagingException>.Final();
                }

                //// TODO figure out async `queryresult`s
                var nextContext = this.contextGenerator(this.graphCalendarEventsResponse.NextPage);
                return Page(nextContext, this.graphCalendarEventsResponse.NextPage, this.contextGenerator).ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphCalendarEventsContext"></param>
        /// <param name="graphQuery">TODO this allows <paramref name="graphQuery"/> to be a paging query; do you want to protect against that for some reason?</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphCalendarEventsContext"/> or <paramref name="graphQuery"/> is <see langword="null"</exception>
        public static async Task<QueryResult<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationException>, GraphPagingException>> Page(
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
            
            //// TODO you are here
            return await graphCalendarEventsContext.Page(graphQuery, page => graphCalendarEventsContext).ConfigureAwait(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphCalendarEventsContext"></param>
        /// <param name="graphQuery">TODO this allows <paramref name="graphQuery"/> to be a paging query; do you want to protect against that for some reason?</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="graphCalendarEventsContext"/> or <paramref name="graphQuery"/> or <paramref name="contextGenerator"/> is <see langword="null"</exception>
        public static async Task<QueryResult<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationException>, GraphPagingException>> Page(
            this IGraphCalendarEventsContext graphCalendarEventsContext,
            GraphQuery graphQuery,
            Func<GraphQuery.Page, IGraphCalendarEventsContext> contextGenerator)
        {
            if (graphCalendarEventsContext == null)
            {
                throw new ArgumentNullException(nameof(graphCalendarEventsContext));
            }

            if (graphQuery == null)
            {
                throw new ArgumentNullException(nameof(graphQuery));
            }

            if (contextGenerator == null)
            {
                throw new ArgumentNullException(nameof(contextGenerator));
            }

            GraphCalendarEventsResponse response;
            try
            {
                //// TODO you are here
                response = await graphCalendarEventsContext.Evaluate(graphQuery).ConfigureAwait(false);
            }
            catch
            {
                return new QueryResult<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationException>, GraphPagingException>.Partial(new GraphPagingException("TODO preserve exception"));
            }

            if (response.Events.Count == 0)
            {
                if (response.NextPage == null)
                {
                    return new QueryResult<Either<GraphCalendarEvent, GraphCalendarEventsContextTranslationException>, GraphPagingException>.Final();
                }
                else
                {
                    var nextContext = contextGenerator(response.NextPage);

                    return await Page(nextContext, response.NextPage, contextGenerator).ConfigureAwait(false);
                }
            }

            return new PageQueryResult(response, 0, graphCalendarEventsContext, contextGenerator);
        }
    }
}
