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
using System.Text;
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
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> is <see langword="null"/></exception>
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
        /// <summary>
        /// TODO remove this once you have this whole class and its related types as ASTs
        /// </summary>
        public static OdataServiceRoot MicrosoftGraph { get; } = new OdataServiceRoot.WithoutPort(
            OdataNextLink.Inners.Scheme.Https.Instance,
            new OdataNextLink.Inners.Host("graph.microsoft.com"),
            new[]
            {
                new OdataNextLink.Inners.Segment("v1.0"),
            });

        private OdataServiceRoot()
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
        /// <exception cref="Exception">Throws any of the exceptions that the <see cref="Visitor{TResult, TContext}.Accept"/> overloads can throw</exception> //// TODO is this good?
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Throws any of the exceptions that the <see cref="Accept"/> overloads can throw</exception> //// TODO is this good?
            public TResult Visit(OdataServiceRoot node, TContext context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                return node.Dispatch(this, context);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            protected internal abstract TResult Accept(WithPort node, TContext context);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
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

            /// <inheritdoc/>
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

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

            /// <inheritdoc/>
            protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return visitor.Accept(this, context);
            }
        }
    }

    public static class OdataServiceRootExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="odataServiceRoot"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="odataServiceRoot"/> is <see langword="null"/></exception>
        public static AbsoluteUri ToAbsoluteUri(this OdataServiceRoot odataServiceRoot)
        {
            if (odataServiceRoot == null)
            {
                throw new ArgumentNullException(nameof(odataServiceRoot));
            }

            //// TODO share transcribers with the nextlink extensions below
            //// TODO you are here
            var uri = OdataServiceRootTranscriber.Instance.Visit(odataServiceRoot, default);
            return new Uri(uri, UriKind.Absolute).ToAbsoluteUri();
        }

        private sealed class OdataServiceRootTranscriber : OdataServiceRoot.Visitor<string, Void>
        {
            /// <summary>
            /// 
            /// </summary>
            private OdataServiceRootTranscriber()
            {
            }

            /// <summary>
            /// 
            /// </summary>
            public static OdataServiceRootTranscriber Instance { get; } = new OdataServiceRootTranscriber();

            /// <inheritdoc/>
            protected internal override string Accept(OdataServiceRoot.WithPort node, Void context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                //// TODO you are here
                var scheme = SchemeTranscriber.Instance.Visit(node.Scheme, context);
                var segments = SegmentsTranscriber.Instance.Transcribe(node.Segments);
                return $"{scheme}://{node.Host.Value}:{node.Port}/{segments}";
            }

            /// <inheritdoc/>
            protected internal override string Accept(OdataServiceRoot.WithoutPort node, Void context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                //// TODO you are here
                var scheme = SchemeTranscriber.Instance.Visit(node.Scheme, context);
                var segments = SegmentsTranscriber.Instance.Transcribe(node.Segments);
                return $"{scheme}://{node.Host.Value}/{segments}";
            }

            private sealed class SchemeTranscriber : OdataNextLink.Inners.Scheme.Visitor<string, Void>
            {
                /// <summary>
                /// 
                /// </summary>
                private SchemeTranscriber()
                {
                }

                /// <summary>
                /// 
                /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="odataServiceRoot"></param>
        /// <param name="odataNextLink"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="odataServiceRoot"/> or <paramref name="odataNextLink"/> is <see langword="null"/></exception>
        internal static RelativeUri GetUri(this OdataServiceRoot odataServiceRoot, OdataNextLink.Absolute odataNextLink)
        {
            if (odataServiceRoot == null)
            {
                throw new ArgumentNullException(nameof(odataServiceRoot));
            }

            if (odataNextLink == null)
            {
                throw new ArgumentNullException(nameof(odataNextLink));
            }

            return odataServiceRoot
            //// TODO you are here
                .ToAbsoluteUri()
                .MakeRelativeUri(
                    odataNextLink
                        .ToAbsoluteUri())
                .ToRelativeUri();
        }
    }

    public static class OdataNextLinkExtensions
    {
        public static AbsoluteUri ToAbsoluteUri(this OdataNextLink.Absolute odataNextLink)
        {
            //// TODO use stringbuilder
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
                    //// TODO re-use class from relative uri
                    return string.Join("/", segments.Select(segment => segment.Value));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="odataNextLink"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="odataNextLink"/> is <see langword="null"/></exception>
        public static RelativeUri ToRelativeUri(this OdataNextLink.Relative odataNextLink)
        {
            if (odataNextLink == null)
            {
                throw new ArgumentNullException(nameof(odataNextLink));
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append("/");
            SegmentsTranscriber.Instance.Transcribe(odataNextLink.Segments, stringBuilder);

            return new Uri(stringBuilder.ToString(), UriKind.Relative).ToRelativeUri();
        }

        private sealed class SegmentsTranscriber
        {
            /// <summary>
            /// 
            /// </summary>
            private SegmentsTranscriber()
            {
            }

            /// <summary>
            /// 
            /// </summary>
            public static SegmentsTranscriber Instance { get; } = new SegmentsTranscriber();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="segments"></param>
            /// <param name="builder"></param>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="segments"/> or <paramref name="builder"/> is <see langword="null"/></exception>
            public void Transcribe(IEnumerable<OdataNextLink.Inners.Segment> segments, StringBuilder builder)
            {
                //// TODO you should probably establish an interface for transcribers (pull from other repo?)
                //// TODO you could then have an extension that `tostring`s a transcriber
                //// TODO should transcribers return the stringbuilder?
                if (segments == null)
                {
                    throw new ArgumentNullException(nameof(segments));
                }

                if (builder == null)
                {
                    throw new ArgumentNullException(nameof(builder));
                }

                builder.AppendJoin("/", segments.Select(segment => segment?.Value)); //// TODO you are being very strict here; technically, the link constructor hasn't asserted that there are no null elements; i don't know if this is worth it
            }
        }
    }

    public abstract class OdataNextLink
    {
        private OdataNextLink()
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
        /// <exception cref="Exception">Throws any of the exceptions that the <see cref="Visitor{TResult, TContext}.AcceptAsync"/> overloads can throw</exception> //// TODO is this good?
        protected abstract Task<TResult> DispatchAsync<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public OdataNextLink AsBase()
        {
            //// TODO extension method?
            return this;
        }

        public abstract class Visitor<TResult, TContext>
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Throws any of the exceptions that the <see cref="AcceptAsync"/> overloads can throw</exception> //// TODO is this good?
            public async Task<TResult> VisitAsync(OdataNextLink node, TContext context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                return await node.DispatchAsync(this, context).ConfigureAwait(false);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            protected internal abstract Task<TResult> AcceptAsync(Null node, TContext context);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            protected internal abstract Task<TResult> AcceptAsync(Relative node, TContext context);

            /// <summary>
            /// 
            /// </summary>
            /// <param name="node"></param>
            /// <param name="context"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
            /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
            protected internal abstract Task<TResult> AcceptAsync(Absolute node, TContext context);
        }

        public sealed class Null : OdataNextLink
        {
            private Null()
            {
            }

            public static Null Instance { get; } = new Null();

            /// <inheritdoc/>
            protected sealed override async Task<TResult> DispatchAsync<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

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

            /// <summary>
            /// NOTE: may contain segments that are <see cref="string.Empty"/>
            /// </summary>
            public IEnumerable<Inners.Segment> Segments { get; }

            /// <inheritdoc/>
            protected sealed override async Task<TResult> DispatchAsync<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

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

            /// <inheritdoc/>
            protected sealed override async Task<TResult> DispatchAsync<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return await visitor.AcceptAsync(this, context).ConfigureAwait(false);
            }
        }

        //// TODO i really don't like this name
        public static class Inners
        {
            public sealed class Segment
            {
                /// <summary>
                /// 
                /// </summary>
                /// <param name="value"></param>
                /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <see langword="null"/></exception>
                /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not a valid URI segment</exception>
                internal Segment(string value)
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(value));
                    }

                    if (value.Contains("/") || value.Contains("?") || value.Contains("#"))
                    {
                        throw new ArgumentException($"'{nameof(value)}' is not a valid URI segment. The provided was value '{value}'.");
                    }

                    try
                    {
                        new Uri(value);
                    }
                    catch (UriFormatException uriFormatException)
                    {
                        throw new ArgumentException($"'{nameof(value)}' is not a valid URI segment. The provided was value '{value}'.", uriFormatException);
                    }

                    Value = value;
                }

                /// <summary>
                /// NOTE: may be <see cref="string.Empty"/> if it represents the "segment" between two contiguous '/' characters
                /// </summary>
                internal string Value { get; }
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
                    /// <summary>
                    /// 
                    /// </summary>
                    /// <param name="scheme"></param>
                    /// <param name="host"></param>
                    /// <param name="port"></param>
                    /// <param name="segments"></param>
                    /// <exception cref="ArgumentNullException">Thrown if <paramref name="scheme"/> or <paramref name="host"/> or <paramref name="segments"/> is <see langword="null"/></exception>
                    public WithPort(Inners.Scheme scheme, Inners.Host host, uint port, IEnumerable<Segment> segments)
                    {
                        if (scheme == null)
                        {
                            throw new ArgumentNullException(nameof(scheme));
                        }

                        if (host == null)
                        {
                            throw new ArgumentNullException(nameof(host));
                        }

                        if (segments == null)
                        {
                            throw new ArgumentNullException(nameof(segments));
                        }

                        this.Scheme = scheme;
                        this.Host = host;
                        this.Port = port;
                        this.Segments = segments;
                    }

                    public Scheme Scheme { get; }
                    public Host Host { get; }
                    public uint Port { get; }

                    /// <summary>
                    /// NOTE: may contain segments that are <see cref="string.Empty"/>
                    /// </summary>
                    public IEnumerable<Segment> Segments { get; }

                    protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                    {
                        return visitor.Accept(this, context);
                    }
                }

                public sealed class WithoutPort : AbsoluteNextLink
                {
                    /// <summary>
                    /// 
                    /// </summary>
                    /// <param name="scheme"></param>
                    /// <param name="host"></param>
                    /// <param name="segments"></param>
                    /// <exception cref="ArgumentNullException">Thrown if <paramref name="scheme"/> or <paramref name="host"/> or <paramref name="segments"/> is <see langword="null"/></exception>
                    public WithoutPort(Inners.Scheme scheme, Inners.Host host, IEnumerable<Segment> segments)
                    {
                        if (scheme == null)
                        {
                            throw new ArgumentNullException(nameof(scheme));
                        }

                        if (host == null)
                        {
                            throw new ArgumentNullException(nameof(host));
                        }

                        if (segments == null)
                        {
                            throw new ArgumentNullException(nameof(segments));
                        }

                        this.Scheme = scheme;
                        this.Host = host;
                        this.Segments = segments;
                    }

                    public Scheme Scheme { get; }
                    public Host Host { get; }

                    /// <summary>
                    /// NOTE: may contain segments that are <see cref="string.Empty"/>
                    /// </summary>
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

                /// <summary>
                /// 
                /// </summary>
                /// <typeparam name="TResult"></typeparam>
                /// <typeparam name="TContext"></typeparam>
                /// <param name="visitor"></param>
                /// <param name="context"></param>
                /// <returns></returns>
                /// <exception cref="ArgumentNullException">Thrown if <paramref name="visitor"/> is <see langword="null"/></exception>
                /// <exception cref="Exception">Throws any of the exceptions that the <see cref="Visitor{TResult, TContext}.Accept"/> overloads can throw</exception> //// TODO is this good?
                protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

                public abstract class Visitor<TResult, TContext>
                {
                    /// <summary>
                    /// 
                    /// </summary>
                    /// <param name="node"></param>
                    /// <param name="context"></param>
                    /// <returns></returns>
                    /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
                    /// <exception cref="Exception">Throws any of the exceptions that the <see cref="Accept"/> overloads can throw</exception> //// TODO is this good?
                    public TResult Visit(Scheme node, TContext context)
                    {
                        if (node == null)
                        {
                            throw new ArgumentNullException(nameof(node));
                        }

                        return node.Dispatch(this, context);
                    }

                    /// <summary>
                    /// 
                    /// </summary>
                    /// <param name="node"></param>
                    /// <param name="context"></param>
                    /// <returns></returns>
                    /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
                    /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
                    protected internal abstract TResult Accept(Https node, TContext context);

                    /// <summary>
                    /// 
                    /// </summary>
                    /// <param name="node"></param>
                    /// <param name="context"></param>
                    /// <returns></returns>
                    /// <exception cref="ArgumentNullException">Thrown if <paramref name="node"/> is <see langword="null"/></exception>
                    /// <exception cref="Exception">Can throw any exception as documented by the derived type</exception>
                    protected internal abstract TResult Accept(Http node, TContext context);
                }

                public sealed class Https : Scheme
                {
                    private Https()
                    {
                    }

                    public static Https Instance { get; } = new Https();

                    /// <inheritdoc/>
                    protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                    {
                        if (visitor == null)
                        {
                            throw new ArgumentNullException(nameof(visitor));
                        }

                        return visitor.Accept(this, context);
                    }
                }

                public sealed class Http : Scheme
                {
                    private Http()
                    {
                    }

                    public static Http Instance { get; } = new Http();

                    /// <inheritdoc/>
                    protected sealed override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
                    {
                        if (visitor == null)
                        {
                            throw new ArgumentNullException(nameof(visitor));
                        }

                        return visitor.Accept(this, context);
                    }
                }
            }

            public sealed class Host
            {
                /// <summary>
                /// TODO this should be modeled as an AST; remove <see cref="OdataServiceRoot.MicrosoftGraph"/> was you have modeled it that way
                /// </summary>
                /// <param name="value"></param>
                /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <see langword="null"/></exception>
                /// <exception cref="ArgumentException">Thrown if <paramref name="value"/> is not a valid host name</exception>
                internal Host(string value)
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(value));
                    }

                    if (string.IsNullOrEmpty(value))
                    {
                        throw new ArgumentException(nameof(value));
                    }

                    if (value.Contains("/") || value.Contains(":") || value.Contains("?") || value.Contains("#"))
                    {
                        throw new ArgumentException($"'{nameof(value)}' is not a valid host name. The provided was value '{value}'.");
                    }

                    try
                    {
                        new Uri(value);
                    }
                    catch (UriFormatException uriFormatException)
                    {
                        throw new ArgumentException($"'{nameof(value)}' is not a valid host name. The provided was value '{value}'.", uriFormatException);
                    }

                    Value = value;
                }

                internal string Value { get; }
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

                var odataNextLink = Parse(this.NextLink);

                return odataNextLink.SelectLeft(left => 
                    new OdataCollectionResponse.Values(
                        //// TODO see if you can avoid the bang
                        this.Value!.Select(jsonNode => new OdataCollectionValue.Json(jsonNode)).ToList(), //// TODO do you want to make this list lazy?
                        left)
                    .AsBase());
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="nextLink"></param>
            /// <returns></returns>
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
                    //// TODO i think your exception parameter pattern should be: exception specific stuff, message, inner
                    return Either.Left<OdataNextLink>().Right(new OdataSuccessDeserializationException("TODO", uriFormatException, "TODO"));
                }

                if (uri.IsAbsoluteUri)
                {
                    var schemeDelimiter = "://";
                    var schemeDelimiterIndex = nextLink.IndexOf(schemeDelimiter, 0);
                    if (schemeDelimiterIndex < 0)
                    {
                        return Either.Left<OdataNextLink>().Right(new OdataSuccessDeserializationException("TODO", "TODO"));
                    }

                    var providedScheme = Substring2(uri.OriginalString, 0, schemeDelimiterIndex); // we know that it's a valid URI, so if the scheme delimiter is present, there must be a scheme
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
                        return Either.Left<OdataNextLink>().Right(new OdataSuccessDeserializationException("TODO", "TODO"));
                    }

                    var hostDelimiter = "/";
                    var hostDelimiterIndex = uri.OriginalString.IndexOf(hostDelimiter, schemeDelimiterIndex + schemeDelimiter.Length); // we know it's a valid URI, so if there was a scheme, there must be a host
                    if (hostDelimiterIndex < 0)
                    {
                        return Either.Left<OdataNextLink>().Right(new OdataSuccessDeserializationException("TODO", "TODO"));
                    }

                    var fullHost = Substring2(uri.OriginalString, schemeDelimiterIndex + schemeDelimiter.Length, hostDelimiterIndex); // we know it's a valid URI, so if there was a scheme, there must be a host
                    var portDelimiter = ":";
                    var portDelimiterIndex = fullHost.IndexOf(portDelimiter, 0);
                    uint? port;
                    OdataNextLink.Inners.Host host;
                    if (portDelimiterIndex < 0)
                    {
                        host = new OdataNextLink.Inners.Host(fullHost);
                        port = null;
                    }
                    else
                    {
                        var providedHost = Substring2(fullHost, 0, portDelimiterIndex); // we know it's a valid URI, so if there's a port delimiter, there must be a host
                        host = new OdataNextLink.Inners.Host(providedHost);

                        var portStartIndex = portDelimiterIndex + portDelimiter.Length;
                        if (portStartIndex == fullHost.Length)
                        {
                            // it's legal for a URI to have a port delimiter without a port
                            return Either.Left<OdataNextLink>().Right(new OdataSuccessDeserializationException("TODO", "TODO"));
                        }

                        var providedPort = Substring2(fullHost, portStartIndex, fullHost.Length);
                        try
                        {
                            port = uint.Parse(providedPort);
                        }
                        catch
                        {
                            return Either.Left<OdataNextLink>().Right(new OdataSuccessDeserializationException("TODO", "TODO")); //// TODO preserve exception
                        }
                    }

                    var segmentsStartIndex = hostDelimiterIndex + hostDelimiter.Length;
                    if (segmentsStartIndex == uri.OriginalString.Length)
                    {
                        return Either.Left<OdataNextLink>().Right(new OdataSuccessDeserializationException("TODO this is a legal service root, but not a legal nextlink", "TODO"));
                    }

                    var providedRelativeUri = Substring2(uri.OriginalString, segmentsStartIndex, uri.OriginalString.Length);

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
                                            segment)))
                                .AsBase());
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="uri"></param>
            /// <returns>NOTE: might contain <see cref="string.Empty"/> elements if there are contiguous '/' characters</returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="uri"/> is <see langword="null"/></exception>
            private static IEnumerable<string> ParseSegments(string uri)
            {
                if (uri == null)
                {
                    throw new ArgumentNullException(nameof(uri));
                }

                var segmentDelimiter = "/";
                var segments = new List<string>();
                var lastIndex = 0;
                while (true)
                {
                    var segmentDelimiterIndex = uri.IndexOf(segmentDelimiter, lastIndex);
                    if (segmentDelimiterIndex < 0)
                    {
                        yield return Substring2(uri, lastIndex, uri.Length);
                        break;
                    }

                    if (lastIndex == segmentDelimiterIndex)
                    {
                        yield return string.Empty;
                    }
                    else
                    {
                        yield return Substring2(uri, lastIndex, segmentDelimiterIndex);
                    }
                    lastIndex = segmentDelimiterIndex + segmentDelimiter.Length;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="value"></param>
            /// <param name="inclusiveStartIndex"></param>
            /// <param name="exclusiveEndIndex"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is <see langword="null"/></exception>
            /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="inclusiveStartIndex"/> is negative, <paramref name="exclusiveEndIndex"/> is less than or equal to <paramref name="inclusiveStartIndex"/>, or <paramref name="exclusiveEndIndex"/> is larger than the <see cref="string.Length"/> or <paramref name="value"/></exception>
            private static string Substring2(string value, int inclusiveStartIndex, int exclusiveEndIndex)
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (inclusiveStartIndex < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(inclusiveStartIndex), $"'{nameof(inclusiveStartIndex)}' cannot be negative. The provided value was '{inclusiveStartIndex}'.");
                }

                if (exclusiveEndIndex <= inclusiveStartIndex)
                {
                    throw new ArgumentOutOfRangeException(nameof(exclusiveEndIndex), $"'{nameof(exclusiveEndIndex)}' must be greater than '{nameof(inclusiveStartIndex)}'. The provided end index was '{exclusiveEndIndex}'. The provided start index was '{inclusiveStartIndex}'.");
                }

                if (exclusiveEndIndex > value.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(exclusiveEndIndex), $"'{nameof(exclusiveEndIndex)}' cannot be greater than the length of the string. The provided index was '{exclusiveEndIndex}'. The length of the string was '{value.Length}'.");
                }

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
