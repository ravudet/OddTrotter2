////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.RequestBuilder
{
    public delegate IGetCollectionRequestBuilder<T> Unit<T>(IGetCollectionRequestBuilder builder);

    public interface IGetCollectionRequestBuilder<T>
    {
        IGetCollectionRequestBuilder Builder { get; }

        Unit<T> Unit { get; }
    }
}
