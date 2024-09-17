////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestBuilder
{
    public sealed class GetInstanceRequestBuilder<T> : IGetInstanceRequestBuilder<T>
    {
        public GetInstanceRequestBuilder(IGetInstanceRequestBuilder getInstanceRequestBuilder)
        {
            this.Builder = getInstanceRequestBuilder;
        }

        public OdataRequest.GetInstance Request
        {
            get
            {
                return this.Builder.Request;
            }
        }

        public IGetInstanceRequestBuilder Builder { get; }

        public GetInstanceUnit<T> Unit
        {
            get
            {
                return
                    (IGetInstanceRequestBuilder getInstanceRequestBuilder) =>
                        new GetInstanceRequestBuilder<T>(getInstanceRequestBuilder);
            }
        }
    }
}
