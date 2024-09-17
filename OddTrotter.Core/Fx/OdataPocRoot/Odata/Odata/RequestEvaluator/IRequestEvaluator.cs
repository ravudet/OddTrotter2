////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestEvaluator
{
    using System.Threading.Tasks;

    public interface IRequestEvaluator
    {
        Task<OdataResponse.Instance> Evaluate(OdataRequest.GetInstance request);

        Task<OdataResponse.Collection> Evaluate(OdataRequest.GetCollection request);
    }
}
