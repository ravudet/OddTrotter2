////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata
{
    public abstract class OdataRequest<T>
    {
        private OdataRequest()
        {
        }

        public sealed class GetInstance
        {
            public GetInstance(OdataRequest.GetInstance request)
            {
                //// TODO do you like this pattern?
                Request = request;
            }

            public OdataRequest.GetInstance Request { get; }
        }

        public sealed class GetCollection
        {
            public GetCollection(OdataRequest.GetCollection request)
            {
                //// TODO do you like this pattern?
                Request = request;
            }

            public OdataRequest.GetCollection Request { get; }
        }
    }
}
