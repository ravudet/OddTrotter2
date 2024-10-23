////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.RequestBuilder
{
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter;
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Select;
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Top;
    using Fx.OdataPocRoot.V2.System.Uri;

    public sealed class GetCollectionRequestBuilder : IGetCollectionRequestBuilder
    {
        private readonly Path uri;

        private readonly Filter? filter;

        private readonly Select? select;

        private readonly Top? top;

        public GetCollectionRequestBuilder(Path uri)
        {
            this.uri = uri;

            this.filter = null;
            this.select = null;
            this.top = null;
        }

        public Request.GetCollection Request => throw new global::System.NotImplementedException();

        public IGetCollectionRequestBuilder Filter(Filter filter)
        {
            throw new global::System.NotImplementedException();
        }

        public IGetCollectionRequestBuilder Select(Select select)
        {
            throw new global::System.NotImplementedException();
        }

        public IGetCollectionRequestBuilder Top(Top top)
        {
            throw new global::System.NotImplementedException();
        }
    }
}
