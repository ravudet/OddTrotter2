////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestEvaluator
{
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Threading.Tasks;
    using static Fx.OdataPocRoot.Odata.Odata.OdataResponse;

    public static class RequestEvaluatorExtensions
    {
        public static async Task<OdataResponse<T>.GetInstance> Evaluate<T>(
            this IRequestEvaluator requestEvaluator,
            OdataRequest<T>.GetInstance request)
        {
            //// TODO use mixins for this? probably would be nice so callers can override serialization behavior
            
            var response = await requestEvaluator.Evaluate(request.Request).ConfigureAwait(false);

            //// TODO use deserialzier options from context to do this correclt
            var instance = JsonSerializer.Deserialize<T>(response.Contents);
            if (instance == null)
            {
                throw new System.Exception("TODO null instance");
            }

            return new OdataResponse<T>.GetInstance(
                instance, 
                new OdataResponse<T>.GetInstance.InstanceControlInformation());
        }

        public static async Task<OdataResponse<T>.GetCollection> Evaluate<T>(
            this IRequestEvaluator requestEvaluator,
            OdataRequest<T>.GetCollection request)
        {
            var response = await requestEvaluator.Evaluate(request.Request).ConfigureAwait(false);

            //// TODO use deserialzier options from context to do this correclt
            var collection = JsonSerializer.Deserialize<IEnumerable<T>>(response.Contents);
            if (collection == null)
            {
                throw new System.Exception("TODO null collection");
            }

            return new OdataResponse<T>.GetCollection(
                collection,
                new OdataResponse<T>.GetCollection.CollectionControlInformation(null, 0));
        }
    }
}
