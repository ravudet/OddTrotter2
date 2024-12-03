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
        internal OdataGetCollectionRequest(RelativeUri relativeUri)
        {
            this.RelativeUri = relativeUri;
        }

        internal RelativeUri RelativeUri { get; }
    }

    public interface IOdataStructuredContext
    {
        Task<Either<OdataCollectionResponse, OdataErrorResponse>> GetCollection(OdataGetCollectionRequest request); //// TODO you can get a legal odata response from any url, even ones that are not valid odata urls; maybe you should have an adapter from things like odatacollectionrequest to httprequestmessage?
    }

    /*public sealed class OdataObject
    {
        public OdataObject(JsonNode jsonNode)
        {
            //// TODO this should be more AST-looking instead of using JSON
            //// TODO you've named this type odata "object", but could it also be individual values (like in a collection)?
            this.JsonNode = jsonNode;
        }

        public JsonNode JsonNode { get; }
    }*/

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

        public OdataCalendarEventsContext(IOdataClient odataClient)
        {
            this.odataClient = odataClient;
        }

        public async Task<Either<OdataCollectionResponse, OdataErrorResponse>> GetCollection(OdataGetCollectionRequest request)
        {
            HttpResponseMessage? httpResponseMessage = null;
            try
            {
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
