////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.RequestEvaluator
{
    using global::System.Threading.Tasks;

    public interface IRequestEvaluator
    {
        Task<Response> Evaluate(Request.GetCollection request); //// TODO you'll want to make clear that this is *either* a collection response, *or* an error response (also, you need to handle transmission responses such as crashes that resulted in responses that were only partially streamed to the client)

        //// TODO FEATURE GAP evaluations for other request types here
    }
}
