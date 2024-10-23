////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.RequestBuilder
{
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter;
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Select;
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Top;
    using Fx.OdataPocRoot.V2.System.Uri;

    public sealed class GetCollectionRequestBuilder : IGetCollectionRequestBuilder
    {
        public GetCollectionRequestBuilder(Path uri)
        {
            this.Request = new Request.GetCollection(uri, null, null, null);
        }

        public Request.GetCollection Request { get; }

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
