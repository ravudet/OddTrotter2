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
        public IGetInstanceRequestBuilder Builder { get; } //// TODO should this interface *implement* the non-generic one, or should we have this property? having the property follows what you've previously established as the monad pattern in linqv2; if you do that, you should probably remove the "Request" property and let the caller use "Builder.Request" to get it

        public GetInstanceUnit<T> Unit { get; }
    }
}
