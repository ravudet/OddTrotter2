////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata
{
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter;
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Select;
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Top;
    using Fx.OdataPocRoot.V2.System.Uri;

    public abstract class Request
    {
        private Request()
        {
        }

        public sealed class GetCollection : Request
        {
            public GetCollection(Path uri, Filter? filter, Select? select, Top? top)
            {
                this.Uri = uri;
                this.Filter = filter;
                this.Select = select;
                this.Top = top;
            }

            public Path Uri { get; }

            public Filter? Filter { get; }

            public Select? Select { get; }

            public Top? Top { get; }
        }
    }
}
