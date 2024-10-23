////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.RequestEvaluator
{
    using global::System.Threading.Tasks;

    public interface IRequestEvaluator
    {
        Task<Response> Evaluate(Request.GetCollection request);

        //// TODO FEATURE GAP evaluations for other request types here
    }
}
