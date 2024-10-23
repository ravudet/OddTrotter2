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
                //// TODO implement this class correctly

                StatusCode = statusCode;
                Headers = headers;
                Body = body;
            }

            public int StatusCode { get; }

            public IReadOnlyDictionary<string, IEnumerable<string>> Headers { get; }

            public Stream Body { get; }
        }

        //// TODO FEATURE GAP: other response types here
    }
}
