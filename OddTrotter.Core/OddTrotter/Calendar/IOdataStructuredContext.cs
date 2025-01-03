////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.V2;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static OddTrotter.Calendar.OdataNextLink.Inners;
using static OddTrotter.Calendar.OdataNextLink.Inners.AbsoluteNextLink;

namespace OddTrotter.Calendar
{
    public sealed class OdataGetCollectionRequest //// TODO are you sure that this is not a discriminated union?
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativeUri"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="relativeUri"/> or <paramref name="headers"/> is <see langword="null"/></exception>
        internal OdataGetCollectionRequest(RelativeUri relativeUri, IEnumerable<HttpHeader> headers)
        {
            if (relativeUri == null)
            {
                throw new ArgumentNullException(nameof(relativeUri));
            }

            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            this.RelativeUri = relativeUri;
            this.Headers = headers;
        }

        internal RelativeUri RelativeUri { get; }
        internal IEnumerable<HttpHeader> Headers { get; }
    }

    public interface IOdataStructuredContext
    {
        OdataServiceRoot ServiceRoot { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> is <see langword="null"</exception>
        /// <exception cref="HttpRequestException">Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout, or the server responded with a payload that was not valid HTTP</exception> //// TODO this part about the invalid HTTP needs to be added to all of your xmldoc where you get an httprequestexception from httpclient
        /// <exception cref="OdataErrorDeserializationException">Thrown if an error occurred while deserializing the OData error response</exception>
        /// <exception cref="OdataSuccessDeserializationException">Thrown if an error occurred while deserializing the OData success response</exception>
        Task<OdataResponse<OdataCollectionResponse>> GetCollection(OdataGetCollectionRequest request); //// TODO you can get a legal odata response from any url, even ones that are not valid odata urls; maybe you should have an adapter from things like odatacollectionrequest to httprequestmessage?
    }

    public sealed class OdataResponse<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpStatusCode"></param>
        /// <param name="headers"></param>
        /// <param name="responseContent"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="headers"/> or <paramref name="responseContent"/> is <see langword="null"/></exception>
        public OdataResponse(HttpStatusCode httpStatusCode, IEnumerable<HttpHeader> headers, Either<T, OdataErrorResponse> responseContent)
        {
            if (headers == null)
            {
                throw new ArgumentNullException(nameof(headers));
            }

            if (responseContent == null)
            {
                throw new ArgumentNullException(nameof(responseContent));
            }

            HttpStatusCode = httpStatusCode;
            Headers = headers; //// TODO do you want to start documenting that there are no guarantees that the elements aren't null?
            ResponseContent = responseContent;
        }

        public HttpStatusCode HttpStatusCode { get; }
        public IEnumerable<HttpHeader> Headers { get; }
        public Either<T, OdataErrorResponse> ResponseContent { get; }
    }

    public static class OdataCollectionResponseExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public static OdataCollectionResponse AsBase(this OdataCollectionResponse response)
        {
            //// TODO do you want to have a method like this for all discriminated unions? you implemented this method specifically because you are lacking covariance in `either`, bu this might just be helpful overall?
            return response;
        }
    }

    public abstract class OdataCollectionResponse
    {
        private OdataCollectionResponse()
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
        /// <exception cref="Exception">Throws any of the exceptions that the <see cref="Visitor{TResult, TContext}.Dispatch"/> overloads can throw</exception> //// TODO is this good?
        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Throws any of the exceptions that the <see cref="Visitor{TResult, TContext}.Dispatch"/> overloads can throw</exception> //// TODO is this good?
            public TResult Visit(OdataCollectionResponse node, TContext context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                return node.Accept(this, context);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            public abstract TResult Dispatch(OdataCollectionResponse.Values node, TContext context);
        }

        public sealed class Values : OdataCollectionResponse //// TODO change the class name; probably i need to know what other derived types there may be before really deciding on a new name...; maybe i should make it internal in the meantime?
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            /// <param name="nextLink"></param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> or <paramref name="nextLink"/> is <see langword="null"/></exception>
            public Values(IReadOnlyList<OdataCollectionValue> value, OdataNextLink nextLink)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (nextLink == null)
                {
                    throw new ArgumentNullException(nameof(nextLink));
                }

                this.Value = value;
                this.NextLink = nextLink;
            }

            public IReadOnlyList<OdataCollectionValue> Value { get; }

            public OdataNextLink NextLink { get; }

            //// TODO any other properties?

            /// <inheritdoc/>
            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return visitor.Dispatch(this, context);
            }
        }
    }

    public abstract class OdataServiceRoot
    {
        private OdataServiceRoot()
        {
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(OdataServiceRoot node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(WithPort node, TContext context);
            protected internal abstract TResult Accept(WithoutPort node, TContext context);
        }

        public sealed class WithPort : OdataServiceRoot
        {
            public WithPort(OdataNextLink.Inners.Scheme scheme, OdataNextLink.Inners.Host host, uint port, IEnumerable<Segment> segments)
            {
                this.Scheme = scheme;
                this.Host = host;
                this.Port = port;
                this.Segments = segments;
            }

            public Scheme Scheme { get; }
            public Host Host { get; }
            public uint Port { get; }
            public IEnumerable<Segment> Segments { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public sealed class WithoutPort : OdataServiceRoot
        {
            public WithoutPort(OdataNextLink.Inners.Scheme scheme, OdataNextLink.Inners.Host host, IEnumerable<Segment> segments)
            {
                this.Scheme = scheme;
                this.Host = host;
                this.Segments = segments;
            }

            public Scheme Scheme { get; }
            public Host Host { get; }
            public IEnumerable<Segment> Segments { get; }

            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }

    public static class OdataServiceRootExtensions
    {
        public static AbsoluteUri ToAbsoluteUri(this OdataServiceRoot odataServiceRoot)
        {
            var uri = OdataServiceRootTranscriber.Instance.Visit(odataServiceRoot, default);
            return new Uri(uri, UriKind.Absolute).ToAbsoluteUri();
        }

        private sealed class OdataServiceRootTranscriber : OdataServiceRoot.Visitor<string, Void>
        {
            private OdataServiceRootTranscriber()
            {
            }

            public static OdataServiceRootTranscriber Instance { get; } = new OdataServiceRootTranscriber();

            protected internal override string Accept(OdataServiceRoot.WithPort node, Void context)
            {
                var scheme = SchemeTranscriber.Instance.Visit(node.Scheme, context);
                var segments = SegmentsTranscriber.Instance.Transcribe(node.Segments);
                return $"{scheme}://{node.Host.Value}:{node.Port}/{segments}";
            }

            protected internal override string Accept(OdataServiceRoot.WithoutPort node, Void context)
            {
                var scheme = SchemeTranscriber.Instance.Visit(node.Scheme, context);
                var segments = SegmentsTranscriber.Instance.Transcribe(node.Segments);
                return $"{scheme}://{node.Host.Value}/{segments}";
            }

            private sealed class SchemeTranscriber : OdataNextLink.Inners.Scheme.Visitor<string, Void>
            {
                private SchemeTranscriber()
                {
                }

                public static SchemeTranscriber Instance { get; } = new SchemeTranscriber();

                protected internal override string Accept(Scheme.Https node, Void context)
                {
                    return "https";
                }

                protected internal override string Accept(Scheme.Http node, Void context)
                {
                    return "http";
                }
            }

            private sealed class SegmentsTranscriber
            {
                private SegmentsTranscriber()
                {
                }

                public static SegmentsTranscriber Instance { get; } = new SegmentsTranscriber();

                public string Transcribe(IEnumerable<OdataNextLink.Inners.Segment> segments)
                {
                    return string.Join("/", segments.Select(segment => segment.Value));
                }
            }
        }

        internal static RelativeUri GetUri(this OdataServiceRoot odataServiceRoot, OdataNextLink.Absolute odataNextLink)
        {
            return odataServiceRoot.ToAbsoluteUri().MakeRelativeUri(odataNextLink.ToAbsoluteUri()).ToRelativeUri();
        }
    }

    public static class OdataNextLinkExtensions
    {
        public static AbsoluteUri ToAbsoluteUri(this OdataNextLink.Absolute odataNextLink)
        {
            var uri = InnerTranscriber.Instance.Visit(odataNextLink.AbsoluteNextLink, default);
            return new Uri(uri, UriKind.Absolute).ToAbsoluteUri();
        }

        private sealed class InnerTranscriber : OdataNextLink.Inners.AbsoluteNextLink.Visitor<string, Void>
        {
            private InnerTranscriber()
            {
            }

            public static InnerTranscriber Instance { get; } = new InnerTranscriber();

            protected internal override string Accept(OdataNextLink.Inners.AbsoluteNextLink.WithPort node, Void context)
            {
                var scheme = SchemeTranscriber.Instance.Visit(node.Scheme, context);
                var segments = SegmentsTranscriber.Instance.Transcribe(node.Segments);
                return $"{scheme}://{node.Host.Value}:{node.Port}/{segments}";
            }

            protected internal override string Accept(OdataNextLink.Inners.AbsoluteNextLink.WithoutPort node, Void context)
            {
                var scheme = SchemeTranscriber.Instance.Visit(node.Scheme, context);
                var segments = SegmentsTranscriber.Instance.Transcribe(node.Segments);
                return $"{scheme}://{node.Host.Value}/{segments}";
            }

            private sealed class SchemeTranscriber : OdataNextLink.Inners.Scheme.Visitor<string, Void>
            {
                private SchemeTranscriber()
                {
                }

                public static SchemeTranscriber Instance { get; } = new SchemeTranscriber();

                protected internal override string Accept(Scheme.Https node, Void context)
                {
                    return "https";
                }

                protected internal override string Accept(Scheme.Http node, Void context)
                {
                    return "http";
                }
            }

            private sealed class SegmentsTranscriber
            {
                private SegmentsTranscriber()
                {
                }

                public static SegmentsTranscriber Instance { get; } = new SegmentsTranscriber();

                public string Transcribe(IEnumerable<OdataNextLink.Inners.Segment> segments)
                {
                    return string.Join("/", segments.Select(segment => segment.Value));
                }
            }
        }
    }

    public abstract class OdataNextLink
    {
        private OdataNextLink()
        {
        }

        protected abstract Task<TResult> DispatchAsync<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public OdataNextLink AsBase()
        {
            //// TODO extension method?
            return this;
        }

        public abstract class Visitor<TResult, TContext>
        {
            public async Task<TResult> VisitAsync(OdataNextLink node, TContext context)
            {
                return await node.DispatchAsync(this, context).ConfigureAwait(false);
            }

            protected internal abstract Task<TResult> AcceptAsync(Null node, TContext context);
            protected internal abstract Task<TResult> AcceptAsync(Relative node, TContext context);
            protected internal abstract Task<TResult> AcceptAsync(Absolute node, TContext context);
        }

        public sealed class Null : OdataNextLink
        {
            private Null()
            {
            }

            public static Null Instance { get; } = new Null();

            protected sealed override async Task<TResult> DispatchAsync<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return await visitor.AcceptAsync(this, context).ConfigureAwait(false);
            }
        }

        public sealed class Relative : OdataNextLink
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="segments"></param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="segments"/> is <see langword="null"/></exception>
            public Relative(IEnumerable<Inners.Segment> segments)
            {
                if (segments == null)
                {
                    throw new ArgumentNullException(nameof(segments));
                }

                //// TODO what about queryoptions and fragments?
                this.Segments = segments;
            }

            public IEnumerable<Inners.Segment> Segments { get; }

            protected sealed override async Task<TResult> DispatchAsync<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return await visitor.AcceptAsync(this, context).ConfigureAwait(false);
            }
        }

        public sealed class Absolute : OdataNextLink
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="absoluteNextLink"></param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="absoluteNextLink"/> is <see langword="null"/></exception>
            public Absolute(Inners.AbsoluteNextLink absoluteNextLink)
            {
                if (absoluteNextLink == null)
                {
                    throw new ArgumentNullException(nameof(absoluteNextLink));
                }

                //// TODO what about queryoptions and fragments?
                this.AbsoluteNextLink = absoluteNextLink;
            }

            public Inners.AbsoluteNextLink AbsoluteNextLink { get; }

            protected sealed override async Task<TResult> DispatchAsync<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return await visitor.AcceptAsync(this, context).ConfigureAwait(false);
            }
        }

        //// TODO i really don't like this name
        public static class Inners
        {
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

            public abstract class AbsoluteNextLink
            {
                private AbsoluteNextLink()
                {
                }

                protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

                public abstract class Visitor<TResult, TContext>
                {
                    public TResult Visit(AbsoluteNextLink node, TContext context)
                    {
                        return node.Dispatch(this, context);
                    }

                    protected internal abstract TResult Accept(WithPort node, TContext context);
                    protected internal abstract TResult Accept(WithoutPort node, TContext context);
                }

                public sealed class WithPort : AbsoluteNextLink
                {
                    public WithPort(Inners.Scheme scheme, Inners.Host host, uint port, IEnumerable<Segment> segments)
                    {
                        this.Scheme = scheme;
                        this.Host = host;
                        this.Port = port;
                        this.Segments = segments;
                    }

                    public Scheme Scheme { get; }
                    public Host Host { get; }
                    public uint Port { get; }
                    public IEnumerable<Segment> Segments { get; }

                    protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                    {
                        return visitor.Accept(this, context);
                    }
                }

                public sealed class WithoutPort : AbsoluteNextLink
                {
                    public WithoutPort(Inners.Scheme scheme, Inners.Host host, IEnumerable<Segment> segments)
                    {
                        this.Scheme = scheme;
                        this.Host = host;
                        this.Segments = segments;
                    }

                    public Scheme Scheme { get; }
                    public Host Host { get; }
                    public IEnumerable<Segment> Segments { get; }

                    protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                    {
                        return visitor.Accept(this, context);
                    }
                }
            }

            public abstract class Scheme
            {
                private Scheme()
                {
                }

                protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

                public abstract class Visitor<TResult, TContext>
                {
                    public TResult Visit(Scheme node, TContext context)
                    {
                        return node.Dispatch(this, context);
                    }

                    protected internal abstract TResult Accept(Https node, TContext context);
                    protected internal abstract TResult Accept(Http node, TContext context);
                }

                public sealed class Https : Scheme
                {
                    private Https()
                    {
                    }

                    public static Https Instance { get; } = new Https();

                    protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                    {
                        return visitor.Accept(this, context);
                    }
                }

                public sealed class Http : Scheme
                {
                    private Http()
                    {
                    }

                    public static Http Instance { get; } = new Http();

                    protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                    {
                        return visitor.Accept(this, context);
                    }
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
        }
    }

    public abstract class OdataCollectionValue
    {
        private OdataCollectionValue()
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
        /// <exception cref="Exception">Throws any of the exceptions that the <see cref="Visitor{TResult, TContext}.Dispatch"/> overloads can throw</exception> //// TODO is this good?
        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Throws any of the exceptions that the <see cref="Visitor{TResult, TContext}.Dispatch"/> overloads can throw</exception> //// TODO is this good?
            public TResult Visit(OdataCollectionValue node, TContext context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                return node.Accept(this, context);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            internal abstract TResult Dispatch(OdataCollectionValue.Json node, TContext context);
        }

        internal sealed class Json : OdataCollectionValue
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            public Json(JsonNode node)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                this.Node = node;
            }

            /// <summary>
            /// GOTCHA: nulls are allowed as elements in some odata collections depending on the EDM model, so make sure to check for that
            /// </summary>
            public JsonNode Node { get; }

            /// <inheritdoc/>
            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return visitor.Dispatch(this, context);
            }
        }
    }

    public sealed class OdataErrorResponse
    {
        public OdataErrorResponse(string code)
        {
            if (code == null)
            {
                throw new ArgumentNullException(nameof(code));
            }

            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("TODO cannot be empty", nameof(code));
            }

            this.Code = code;
        }

        public string Code { get; }

        //// TODO finish implementing this
    }

    public sealed class OdataSuccessDeserializationException : Exception
    {
        public OdataSuccessDeserializationException(string message, string responseContents)
            : base(message)
        {
            this.ResponseContents = responseContents;
        }

        public OdataSuccessDeserializationException(string message, Exception e, string responseContents)
            : base(message, e)
        {
            this.ResponseContents = responseContents;
        }

        public string ResponseContents { get; }
    }

    public sealed class OdataErrorDeserializationException : Exception
    {
        public OdataErrorDeserializationException(string message, string responseContents)
            : base(message)
        {
            this.ResponseContents = responseContents;
        }

        public OdataErrorDeserializationException(string message, Exception innerException, string responseContents)
            : base(message, innerException)
        {
            this.ResponseContents = responseContents;
        }

        public string ResponseContents { get; }
    }

    public sealed class OdataCalendarEventsContext : IOdataStructuredContext
    {
        private readonly IHttpClient httpClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="serviceRoot"/> or <paramref name="httpClient"/> is <see langword="null"/></exception>
        public OdataCalendarEventsContext(OdataServiceRoot serviceRoot, IHttpClient httpClient)
        {
            if (serviceRoot == null)
            {
                throw new ArgumentNullException(nameof(serviceRoot));
            }

            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            this.ServiceRoot = serviceRoot;
            this.httpClient = httpClient;
        }

        public OdataServiceRoot ServiceRoot { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativeUri"></param>
        /// <returns></returns>
        private AbsoluteUri CreateRequestUri(RelativeUri relativeUri)
        {
            return new Uri(this.ServiceRoot.ToAbsoluteUri(), relativeUri).ToAbsoluteUri();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> is <see langword="null"</exception>
        /// <exception cref="HttpRequestException">Thrown if the request failed due to an underlying issue such as network connectivity, DNS failure, server certificate validation or timeout, or the server responded with a payload that was not valid HTTP</exception> //// TODO this part about the invalid HTTP needs to be added to all of your xmldoc where you get an httprequestexception from httpclient
        /// <exception cref="OdataErrorDeserializationException">Thrown if an error occurred while deserializing the OData error response</exception>
        /// <exception cref="OdataSuccessDeserializationException">Thrown if an error occurred while deserializing the OData success response</exception>
        public async Task<OdataResponse<OdataCollectionResponse>> GetCollection(OdataGetCollectionRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            HttpResponseMessage? httpResponseMessage = null;
            try
            {
                httpResponseMessage = await this.httpClient.GetAsync(CreateRequestUri(request.RelativeUri), request.Headers).ConfigureAwait(false);
                var responseContents = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    //// TODO this pattern of deserialization and error handling might be able to leverage an ibuilder and some extensions; look into that...
                    OdataErrorResponseBuilder? odataErrorResponseBuilder;
                    try
                    {
                        odataErrorResponseBuilder = JsonSerializer.Deserialize<OdataErrorResponseBuilder>(responseContents);
                    }
                    catch (JsonException jsonException)
                    {
                        throw new OdataErrorDeserializationException("Could not deserialize the OData error response", jsonException, responseContents);
                    }

                    if (odataErrorResponseBuilder == null)
                    {
                        throw new OdataErrorDeserializationException("Could not deserialize the OData error response", responseContents);
                    }

                    var odataErrorResponse = odataErrorResponseBuilder.Build(responseContents).ThrowRight();

                    return new OdataResponse<OdataCollectionResponse>(
                        httpResponseMessage.StatusCode,
                        httpResponseMessage
                            .Headers
                            .SelectMany(header =>
                                header.Value.Select(value => (header.Key, value)))
                            .Select(header => new HttpHeader(header.Key, header.Key)), // `HttpClient` validates the headers for us, so we don't have to worry about `HttpHeader` throwing
                        Either.Left<OdataCollectionResponse>().Right(odataErrorResponse));
                }

                OdataCollectionResponseBuilder? odataCollectionResponseBuilder;
                try
                {
                    odataCollectionResponseBuilder = JsonSerializer.Deserialize<OdataCollectionResponseBuilder>(responseContents);
                }
                catch (JsonException jsonException)
                {
                    throw new OdataSuccessDeserializationException("Could not deserialize the OData response payload", jsonException, responseContents);
                }

                if (odataCollectionResponseBuilder == null)
                {
                    throw new OdataSuccessDeserializationException("Could not deserialize the OData response payload", responseContents);
                }

                //// TODO you need to make sure you don't overpivot on the `either` stuff; for example, here the builder should just be throwing since you're not doing any looping
                //// TODO with that in mind, do you want to surface a nextlink that may have issues in it so that the caller can at least get the values in such an event?
                var odataCollectionResponse = odataCollectionResponseBuilder.Build(responseContents).ThrowRight();

                return new OdataResponse<OdataCollectionResponse>(
                    httpResponseMessage.StatusCode,
                    httpResponseMessage
                        .Headers
                        .SelectMany(header =>
                            header.Value.Select(value => (header.Key, value)))
                        .Select(header => new HttpHeader(header.Key, header.Key)),  // `HttpClient` validates the headers for us, so we don't have to worry about `HttpHeader` throwing
                    Either.Right<OdataErrorResponse>().Left(odataCollectionResponse));
            }
            finally
            {
                httpResponseMessage?.Dispose();
            }
        }

        private sealed class OdataCollectionResponseBuilder
        {
            [JsonPropertyName("value")]
            public IReadOnlyList<JsonNode>? Value { get; set; }

            [JsonPropertyName("@odata.nextLink")]
            public string? NextLink { get; set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="responseContents"></param>
            /// <returns></returns>
            public Either<OdataCollectionResponse, OdataSuccessDeserializationException> Build(string responseContents)
            {
                var invalidities = new List<string>();
                if (this.Value == null)
                {
                    invalidities.Add($"'{nameof(Value)}' cannot be null");
                }

                if (invalidities.Count > 0)
                {
                    return Either
                        .Left<OdataCollectionResponse>()
                        .Right(
                            new OdataSuccessDeserializationException(
                                $"The response payload was not a valid OData response: {string.Join(", ", invalidities)}", responseContents));
                }

                //// TODO dcouemnt exceptions
                var odataNextLink = Parse(this.NextLink);

                //// TODO document exceptions
                return odataNextLink.SelectLeft(left => 
                    new OdataCollectionResponse.Values(
                        //// TODO see if you can avoid the bang
                        this.Value!.Select(jsonNode => new OdataCollectionValue.Json(jsonNode)).ToList(), //// TODO do you want to make this list lazy?
                        left)
                    .AsBase());
            }

            private static Either<OdataNextLink, OdataSuccessDeserializationException> Parse(string? nextLink)
            {
                if (nextLink == null)
                {
                    return Either
                        .Right<OdataSuccessDeserializationException>()
                        .Left(OdataNextLink.Null.Instance.AsBase());
                }

                //// TODO you need to actually find a proper combinator parsing library instead of all of this string manipulation
                Uri uri;
                try
                {
                    uri = new Uri(nextLink);
                }
                catch (UriFormatException uriFormatException)
                {
                    return Either.Left<OdataNextLink>().Right(new OdataSuccessDeserializationException("TODO", uriFormatException, "TODO"));
                }

                if (uri.IsAbsoluteUri)
                {
                    var schemeDelimiter = "://";
                    var schemeIndex = nextLink.IndexOf(schemeDelimiter, 0);
                    if (schemeIndex < 0)
                    {
                        throw new Exception("TODO");
                    }

                    var providedScheme = Substring2(uri.OriginalString, 0, schemeIndex);
                    OdataNextLink.Inners.Scheme scheme;
                    if (string.Equals(providedScheme, "https", StringComparison.OrdinalIgnoreCase))
                    {
                        scheme = OdataNextLink.Inners.Scheme.Https.Instance;
                    }
                    else if (string.Equals(providedScheme, "http", StringComparison.OrdinalIgnoreCase))
                    {
                        scheme = OdataNextLink.Inners.Scheme.Http.Instance;
                    }
                    else
                    {
                        throw new Exception("TODO");
                    }

                    var hostDelimiter = "/";
                    var hostIndex = uri.OriginalString.IndexOf(hostDelimiter, schemeIndex + 1);
                    if (hostIndex < 0)
                    {
                        throw new Exception("TODO");
                    }

                    var fullHost = Substring2(uri.OriginalString, schemeIndex + schemeDelimiter.Length, hostIndex);
                    var portDelimiter = ":";
                    var portIndex = fullHost.IndexOf(portDelimiter, 0);
                    uint? port;
                    OdataNextLink.Inners.Host host;
                    if (portIndex < 0)
                    {
                        host = new OdataNextLink.Inners.Host(fullHost);
                        port = null;
                    }
                    else
                    {
                        var providedHost = Substring2(fullHost, 0, portIndex);
                        host = new OdataNextLink.Inners.Host(providedHost);

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

                    var providedRelativeUri = Substring2(uri.OriginalString, hostIndex + 1, uri.OriginalString.Length);
                    var segments = ParseSegments(providedRelativeUri)
                        .Select(segment => new OdataNextLink.Inners.Segment(segment));

                    if (port == null)
                    {
                        return Either
                            .Right<OdataSuccessDeserializationException>()
                            .Left(
                                new OdataNextLink.Absolute(
                                    new OdataNextLink.Inners.AbsoluteNextLink.WithoutPort(
                                        scheme,
                                        host,
                                        segments))
                                .AsBase());
                    }
                    else
                    {
                        return Either
                            .Right<OdataSuccessDeserializationException>()
                            .Left(
                                new OdataNextLink.Absolute(
                                    new OdataNextLink.Inners.AbsoluteNextLink.WithPort(
                                        scheme,
                                        host,
                                        port.Value,
                                        segments))
                                .AsBase());

                    }
                }
                else
                {
                    return Either
                            .Right<OdataSuccessDeserializationException>()
                            .Left(
                                new OdataNextLink.Relative(ParseSegments(uri.OriginalString)
                                    .Select(segment =>
                                        new OdataNextLink.Inners.Segment(
                                            segment))).AsBase());
                }
            }

            private static IEnumerable<string> ParseSegments(string uri)
            {
                var segmentDelimiter = "/";
                var segments = new List<string>();
                var lastIndex = 0;
                while (true)
                {
                    var segmentIndex = uri.IndexOf(segmentDelimiter, lastIndex);
                    if (segmentIndex < 0)
                    {
                        yield return Substring2(uri, lastIndex, uri.Length);
                        break;
                    }

                    yield return Substring2(uri, lastIndex, segmentIndex);
                    lastIndex = segmentIndex + segmentDelimiter.Length;
                }
            }

            private static string Substring2(string value, int inclusiveStartIndex, int exclusiveEndIndex)
            {
                return value.Substring(inclusiveStartIndex, exclusiveEndIndex - inclusiveStartIndex);
            }
        }

        private sealed class OdataErrorResponseBuilder
        {
            [JsonPropertyName("code")]
            public string? Code { get; set; }

            //// TODO finish implemting this

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public Either<OdataErrorResponse, OdataErrorDeserializationException> Build(string responseContents)
            {
                var invalidities = new List<string>();
                if (this.Code == null || string.IsNullOrEmpty(this.Code))
                {
                    invalidities.Add($"'{nameof(Code)}' cannot be null or empty");
                }

                if (invalidities.Count > 0)
                {
                    return Either
                        .Left<OdataErrorResponse>()
                        .Right(
                            new OdataErrorDeserializationException(
                                $"The error response was not a valid OData response: {string.Join(", ", invalidities)}",
                                responseContents));
                }

                return Either.Right<OdataErrorDeserializationException>().Left(new OdataErrorResponse(this.Code!)); //// TODO see if you can avoid the bang
            }
        }
    }
}
