////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //// TODO you are here
        Task<OdataResponse<OdataCollectionResponse>> GetCollection(OdataGetCollectionRequest request); //// TODO you can get a legal odata response from any url, even ones that are not valid odata urls; maybe you should have an adapter from things like odatacollectionrequest to httprequestmessage?
    }

    public sealed class OdataResponse<T>
    {
        public OdataResponse(HttpStatusCode httpStatusCode, IEnumerable<HttpHeader> headers, Either<T, OdataErrorResponse> responseContent)
        {
            HttpStatusCode = httpStatusCode;
            Headers = headers;
            ResponseContent = responseContent;
        }

        public HttpStatusCode HttpStatusCode { get; }
        public IEnumerable<HttpHeader> Headers { get; }
        public Either<T, OdataErrorResponse> ResponseContent { get; }
    }

    public abstract class OdataCollectionResponse
    {
        private OdataCollectionResponse()
        {
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(OdataCollectionResponse node, TContext context)
            {
                return node.Accept(this, context);
            }

            public abstract TResult Dispatch(OdataCollectionResponse.Values node, TContext context);
        }

        public sealed class Values : OdataCollectionResponse //// TODO change the class name
        {
            public Values(IReadOnlyList<OdataCollectionValue> value, string? nextLink)
            {
                this.Value = value;
                this.NextLink = nextLink;
            }

            public IReadOnlyList<OdataCollectionValue> Value { get; }

            public string? NextLink { get; }

            //// TODO any other properties?

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Dispatch(this, context);
            }
        }
    }

    public abstract class OdataCollectionValue
    {
        private OdataCollectionValue()
        {
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(OdataCollectionValue node, TContext context)
            {
                return node.Accept(this, context);
            }

            internal abstract TResult Dispatch(OdataCollectionValue.Json node, TContext context);
        }

        internal sealed class Json : OdataCollectionValue
        {
            public Json(JsonNode node)
            {
                Node = node;
            }

            public JsonNode Node { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
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

    public sealed class OdataDeserializationException : Exception
    {
        public OdataDeserializationException(string message)
            : base(message)
        {
        }

        public OdataDeserializationException(string message, Exception e)
            : base(message, e)
        {
            //// TODO other overloads
        }
    }

    public sealed class OdataCalendarEventsContext : IOdataStructuredContext
    {
        private readonly AbsoluteUri rootUri;

        private readonly IHttpClient httpClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="httpClient"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="rootUri"/> or <paramref name="httpClient"/> is <see langword="null"/></exception>
        public OdataCalendarEventsContext(AbsoluteUri rootUri, IHttpClient httpClient)
        {
            if (rootUri == null)
            {
                throw new ArgumentNullException(nameof(rootUri));
            }

            if (httpClient == null)
            {
                throw new ArgumentNullException(nameof(httpClient));
            }

            this.rootUri = rootUri;
            this.httpClient = httpClient;
        }

        private AbsoluteUri CreateRequestUri(RelativeUri relativeUri)
        {
            return new Uri(this.rootUri, relativeUri).ToAbsoluteUri();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> is <see langword="null"</exception>
        public async Task<OdataResponse<OdataCollectionResponse>> GetCollection(OdataGetCollectionRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            HttpResponseMessage? httpResponseMessage = null;
            try
            {
                //// TODO you are here
                httpResponseMessage = await this.httpClient.GetAsync(CreateRequestUri(request.RelativeUri), request.Headers).ConfigureAwait(false);
                if (!httpResponseMessage.IsSuccessStatusCode)
                {
                    //// TODO this pattern of deserialization and error handling might be able to leverage an ibuilder and some extensions; look into that...
                    OdataErrorResponseBuilder? odataErrorResponseBuilder;
                    try
                    {
                        odataErrorResponseBuilder = JsonSerializer.Deserialize<OdataErrorResponseBuilder>(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false));
                    }
                    catch (JsonException jsonException)
                    {
                        throw new OdataDeserializationException("TODO", jsonException); //// TODO should there be a separate exception for errors occurring when deserializing an odata error vs and odata success?
                    }

                    if (odataErrorResponseBuilder == null)
                    {
                        throw new OdataDeserializationException("TODO");
                    }

                    var odataErrorResponse = odataErrorResponseBuilder.Build().ThrowRight();

                    return new OdataResponse<OdataCollectionResponse>(
                        httpResponseMessage.StatusCode,
                        httpResponseMessage
                            .Headers
                            .SelectMany(header =>
                                header.Value.Select(value => (header.Key, value)))
                            .Select(header => new HttpHeader(header.Key, header.Key)),
                        Either.Left<OdataCollectionResponse>().Right(odataErrorResponse));
                }

                OdataCollectionResponseBuilder? odataCollectionResponseBuilder;
                try
                {
                    odataCollectionResponseBuilder = JsonSerializer.Deserialize<OdataCollectionResponseBuilder>(await httpResponseMessage.Content.ReadAsStreamAsync().ConfigureAwait(false));
                }
                catch (JsonException jsonException)
                {
                    throw new OdataDeserializationException("tODO", jsonException);
                }

                if (odataCollectionResponseBuilder == null)
                {
                    throw new OdataDeserializationException("TODO");
                }

                var odataCollectionResponse = odataCollectionResponseBuilder.Build().ThrowRight();

                return new OdataResponse<OdataCollectionResponse>(
                    httpResponseMessage.StatusCode,
                    httpResponseMessage
                        .Headers
                        .SelectMany(header =>
                            header.Value.Select(value => (header.Key, value)))
                        .Select(header => new HttpHeader(header.Key, header.Key)),
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

            public Either<OdataCollectionResponse, OdataDeserializationException> Build()
            {
                if (this.Value == null)
                {
                    return Either.Left<OdataCollectionResponse>().Right(new OdataDeserializationException("TODO"));
                }

                return new Either<OdataCollectionResponse, OdataDeserializationException>.Left(new OdataCollectionResponse.Values(this.Value.Select(jsonNode => new OdataCollectionValue.Json(jsonNode)).ToList(), this.NextLink)); //// TODO you can't use the `either` helper factories because the return type is more general than `success`; should discriminated unions have like a "tobasetype" extension or something? or is this an issue with `either` and covariance?
            }
        }

        private sealed class OdataErrorResponseBuilder
        {
            [JsonPropertyName("code")]
            public string? Code { get; set; }

            //// TODO finish implemting this

            public Either<OdataErrorResponse, OdataDeserializationException> Build()
            {
                if (this.Code == null || string.IsNullOrEmpty(this.Code))
                {
                    return Either.Left<OdataErrorResponse>().Right(new OdataDeserializationException("tODO"));
                }

                return Either.Right<OdataDeserializationException>().Left(new OdataErrorResponse(this.Code));
            }
        }
    }
}
