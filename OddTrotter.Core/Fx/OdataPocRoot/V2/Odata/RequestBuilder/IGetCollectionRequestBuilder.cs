////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.RequestBuilder
{
    public interface IGetCollectionRequestBuilder
    {
        Request.GetCollection Request { get; }

        IGetCollectionRequestBuilder Filter(object query);

        IGetCollectionRequestBuilder Top(object query);

        IGetCollectionRequestBuilder Select(object query);

        //// TODO other query options here...
    }
}
