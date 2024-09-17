////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestBuilder
{
    public sealed class GetCollectionRequestBuilder<T> : IGetCollectionRequestBuilder<T>
    {
        public GetCollectionRequestBuilder(IGetCollectionRequestBuilder getCollectionRequestBuilder)
        {
            this.Builder = getCollectionRequestBuilder;
        }

        public OdataRequest.GetCollection Request
        {
            get
            {
                return this.Builder.Request;
            }
        }

        public IGetCollectionRequestBuilder Builder { get; }

        public GetCollectionUnit<T> Unit
        {
            get
            {
                return
                    (IGetCollectionRequestBuilder getCollectionRequestBuilder) => 
                        new GetCollectionRequestBuilder<T>(getCollectionRequestBuilder);
            }
        }
    }
}
