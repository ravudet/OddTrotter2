////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata
{
    using global::System.Collections.Generic;
    using global::System.IO;

    public abstract class Response
    {
        private Response()
        {
        }

        public sealed class Collection : Response
        {
            public Collection(int statusCode, IReadOnlyDictionary<string, IEnumerable<string>> headers, Stream body)
            {
                //// TODO implement this class correctly or remove it

                StatusCode = statusCode;
                Headers = headers;
                Body = body;
            }

            public int StatusCode { get; }

            public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; }

            public Stream Body { get; }
        }

        public sealed class EntityCollection : Response //// TODO how are you handling the odataversion?
        {
            public EntityCollection(HttpStatusCode httpStatusCode, ResponseHeaders headers)
            {
                this.HttpStatusCode = httpStatusCode;
                this.Headers = headers;
            }

            public HttpStatusCode HttpStatusCode { get; } //// TODO do all status codes make sense for all response types? consider that you'll only really return an entity collection for a successful response

            public ResponseHeaders Headers { get; } //// TODO how can you make the headers class name and the property name both make sense?

            public sealed class ResponseHeaders //// TODO is this really a good way to model the headers? the problem is that the allowable headers are specific to the kind of response, so you'll want a bag of them per derived type of response; also, you're really only adding this class to avoid add "header" as a suffix to the property names
            {
                public ResponseHeaders(string contentType)
                {
                    this.ContentType = contentType;
                }

                public string ContentType { get; } //// TODO strongly type this? also, isn't this dependent on odata version?
            }
        }

        //// TODO FEATURE GAP: other response types here
    }

    public abstract class HttpStatusCode //// TODO move to another file; do you want more namespaces or anything?
    {
        private HttpStatusCode()
        {
        }

        public abstract class Odata : HttpStatusCode
        {
            public sealed class Ok : Odata //// TODO do you really need derived types for this?
            {
                private Ok()
                {
                }

                public static Ok Instance { get; } = new Ok();
            }

            public sealed class Created : Odata
            {
                private Created()
                {
                }

                public static Created Instance { get; } = new Created();
            }
        }

        //// TODO FEATURE GAP other odata response codes here

        public sealed class NonOdata : HttpStatusCode //// TODO is this a good way to represent the customer provided status codes?
        {
            public NonOdata(int statusCode)
            {
                this.StatusCode = statusCode;
            }

            public int StatusCode { get; }
        }
    }
}
