////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestEvaluator
{
    using System.Threading.Tasks;

    public static class RequestEvaluatorExtensions
    {
        public static async Task<OdataResponse<T>.GetInstance> Evaluate<T>(
            this IRequestEvaluator requestEvaluator,
            OdataRequest<T>.GetInstance request)
        {
            var response = await requestEvaluator.Evaluate(request).ConfigureAwait(false);
            return new OdataResponse<T>.GetInstance();
        }

        public static async Task<OdataResponse<T>.GetCollection> Evaluate<T>(
            this IRequestEvaluator requestEvaluator,
            OdataRequest<T>.GetCollection request)
        {
            var response = await requestEvaluator.Evaluate(request).ConfigureAwait(false);
            return new OdataResponse<T>.GetCollection();
        }
    }
}
