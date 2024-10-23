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
            public GetCollection(Path path, Filter? filter, Select? select, Top? top)
            {
                this.Path = path;
                this.Filter = filter;
                this.Select = select;
                this.Top = top;
            }

            public Path Path { get; }

            public Filter? Filter { get; }

            public Select? Select { get; }

            public Top? Top { get; }
        }

        //// TODO FEATURE GAP: other request types here
    }
}
