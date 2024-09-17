////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestBuilder
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Expand;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Select;

    public interface IGetInstanceRequestBuilder
    {
        OdataRequest.GetInstance Request { get; }

        IGetInstanceRequestBuilder Expand(Expand query);

        IGetInstanceRequestBuilder Select(Select query);
    }

    public delegate IGetInstanceRequestBuilder<T> GetInstanceUnit<T>(IGetInstanceRequestBuilder builder);

    public interface IGetInstanceRequestBuilder<T>
    {
        OdataRequest.GetInstance Request { get; }

        public IGetInstanceRequestBuilder Builder { get; } //// TODO should this interface *implement* the non-generic one, or should we have this property?

        public GetInstanceUnit<T> Unit { get; }
    }
}
