////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestEvaluator
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Fx.OdataPocRoot.Odata.Odata.RequestEvaluator.Mixins;

    public sealed class BackendEvaluator
    {
        public BackendEvaluator(string backendUri, IRequestEvaluator evaluator)
        {
            BackendUri = backendUri;
            Evaluator = evaluator;
        }

        public string BackendUri { get; } //// TODO you want to pass through query options and stuff, so you really just want the path portion of the URI, maybe also the schema and domain; are there cases where the backend URI needs specific query options? handling that would likely require parsing the OData specific query options and combining them with the original request

        public IRequestEvaluator Evaluator { get; }
    }

    public sealed class CollectionAggregatorRequestEvaluator : IRequestEvaluator, IGetCollectionMixin
    {
        private readonly IRequestEvaluator defaultEvaluator;
        private readonly IReadOnlyDictionary<string, IEnumerable<BackendEvaluator>> aggregatedEvaluators;

        public CollectionAggregatorRequestEvaluator(
            IRequestEvaluator defaultEvaluator,
            IReadOnlyDictionary<string, IEnumerable<BackendEvaluator>> aggregatedEvaluators)
        {
            this.defaultEvaluator = defaultEvaluator;
            this.aggregatedEvaluators = aggregatedEvaluators;
        }

        public async Task<OdataResponse.Instance> Evaluate(OdataRequest.GetInstance request)
        {
            return await this.defaultEvaluator.Evaluate(request);
        }

        public async Task<OdataResponse.Collection> Evaluate(OdataRequest.GetCollection request)
        {
            //// TODO can you somehow leverage the generic implementation? what would it look like for someone to actually implement that?
            return await this.defaultEvaluator.Evaluate(request);
        }

        public Task<OdataResponse<T>.GetCollection> Evaluate<T>(OdataRequest<T>.GetCollection request)
        {
            request.Request.Uri

            throw new System.NotImplementedException();
        }
    }
}
