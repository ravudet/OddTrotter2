////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestEvaluator.Mixins
{
    using System.Threading.Tasks;

    public interface IGetCollectionMixin
    {
        Task<OdataResponse<T>.GetCollection> Evaluate<T>(OdataRequest<T>.GetCollection request);
    }
}
