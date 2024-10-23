////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.RequestBuilder
{
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter;
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Select;
    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Top;

    public interface IGetCollectionRequestBuilder
    {
        Request.GetCollection Request { get; }

        IGetCollectionRequestBuilder Filter(Filter filter);

        IGetCollectionRequestBuilder Select(Select select);

        IGetCollectionRequestBuilder Top(Top top);

        //// TODO other query options here...
    }
}
