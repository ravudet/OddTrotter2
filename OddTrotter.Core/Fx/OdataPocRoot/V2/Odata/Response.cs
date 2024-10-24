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

        public sealed class EntityCollection : Response
        {

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
