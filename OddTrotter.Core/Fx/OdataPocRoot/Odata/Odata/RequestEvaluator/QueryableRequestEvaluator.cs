////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestEvaluator
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter;

    public sealed class QueryableRequestEvaluator<T> : IRequestEvaluator
    {
        private readonly IQueryable<T> queryable;

        public QueryableRequestEvaluator(IQueryable<T> queryable)
        {
            this.queryable = queryable;
        }

        public Task<OdataResponse.Instance> Evaluate(OdataRequest.GetInstance request)
        {
            //// TODO no idea what this would mean
            throw new System.NotImplementedException();
        }

        public Task<OdataResponse.Collection> Evaluate(OdataRequest.GetCollection request)
        {
            var query = this.queryable;
            if (request.Filter != null)
            {
                query = queryable.Where(Filter<T>(request.Filter));
            }

            //// TODO implement other query options;

            var response = new OdataResponse<T>.GetCollection(query.ToList(), null!);

            //// TODO actually implement this
            return Task.FromResult(new OdataResponse.Collection(System.IO.Stream.Null));
        }

        private static Expression<Func<TElement, bool>> Filter<TElement>(Filter query)
        {
            //// TODO implement this
            return _ => true;
        }
    }
}
