////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.V2;
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
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="relativeUri"/> is <see langword="null"/></exception>
        internal OdataGetCollectionRequest(RelativeUri relativeUri)
        {
            if (relativeUri == null)
            {
                throw new ArgumentNullException(nameof(relativeUri));
            }

            this.RelativeUri = relativeUri;
        }

        internal RelativeUri RelativeUri { get; }
    }

    public interface IOdataStructuredContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //// TODO you are here
        Task<Either<OdataCollectionResponse, OdataErrorResponse>> GetCollection(OdataGetCollectionRequest request); //// TODO you can get a legal odata response from any url, even ones that are not valid odata urls; maybe you should have an adapter from things like odatacollectionrequest to httprequestmessage?
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
        private readonly IOdataClient odataClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="odataClient"></param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="odataClient"/> is <see langword="null"/></exception>
        public OdataCalendarEventsContext(IOdataClient odataClient)
        {
            if (odataClient == null)
            {
                throw new ArgumentNullException(nameof(odataClient));
            }

            this.odataClient = odataClient;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> is <see langword="null"</exception>
        public async Task<Either<OdataCollectionResponse, OdataErrorResponse>> GetCollection(OdataGetCollectionRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            HttpResponseMessage? httpResponseMessage = null;
            try
            {
                //// TODO you are here
                ////
                //// TODO here's some thoughts on the `unauthorizedaccessexception` issue:
                //// That exception is *really* a graph thing. *Graph*, not *odata*, is asking for the `Authorization` header to be provided in the request. Graph *also* sometimes asks for *other* headers in the request, or there is optional functionality from graph that can be obtained through additional headers. Those headers *really* are on a per-request basis, so you need a way for the caller of the `iodatastructuredcontext` to be able to provide headers anyway. If you're already having to allow headers, then there's really no difference between the `iodataclient` doing the auth stuff vs the `igraphcalendarcontext` (and any other graph context) doing the auth stuff. There's *maybe* an argument that the graph contexts will all have to do the header stuff, which is true but can be mitigated through composing some sort of token provider. *But*, even so, there are probably graph contexts out there which will need a different token for different requests (maybe based on permissions or something), so they will probably want to be providing the `authorization` header on a per-request basis as well. 
                //// 
                //// And also, you could do to `iodatastructuredcontext` what you did to `httpclient` where you have a `graphcontext` wrapper that delegates to a `iodatastructuredcontext` and provides the `authorization` header. 
                //// 
                //// If all of these things are reasonable and convincing, then `iodataclient` is really just `ihttpclient` and should be modeled as such.
                httpResponseMessage = await this.odataClient.GetAsync(request.RelativeUri).ConfigureAwait(false);
                try
                {
                    httpResponseMessage.EnsureSuccessStatusCode();
                }
                catch (HttpRequestException)
                {
                    //// TODO how do you preserve the http status code? if you don't use the exception, then use issuccessstatuscode instead

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

                    return Either.Left<OdataCollectionResponse>().Right(odataErrorResponse);
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

                return Either.Right<OdataErrorResponse>().Left(odataCollectionResponse);
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
