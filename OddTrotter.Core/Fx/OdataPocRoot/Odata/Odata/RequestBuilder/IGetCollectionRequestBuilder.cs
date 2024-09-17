////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestBuilder
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Expand;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Select;

    public interface IGetCollectionRequestBuilder
    {
        OdataRequest.GetCollection Request { get; }

        IGetInstanceRequestBuilder Expand(Expand query);

        IGetCollectionRequestBuilder Filter(Filter query);

        IGetCollectionRequestBuilder Select(Select query);
    }

    public delegate IGetCollectionRequestBuilder<T> GetCollectionUnit<T>(IGetCollectionRequestBuilder builder);

    public interface IGetCollectionRequestBuilder<T>
    {
        OdataRequest.GetCollection Request { get; } //// TODO this should be generic

        public IGetCollectionRequestBuilder Builder { get; }

        public GetCollectionUnit<T> Unit { get; }
    }
}
