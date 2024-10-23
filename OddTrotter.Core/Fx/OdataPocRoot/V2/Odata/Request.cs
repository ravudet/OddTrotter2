////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata
{
    using Fx.OdataPocRoot.V2.System.Uri;

    public abstract class Request
    {
        private Request()
        {
        }

        public sealed class GetCollection : Request
        {
            public GetCollection(Path uri)
            {
                this.Uri = uri;
            }

            public Path Uri { get; }
        }
    }
}
