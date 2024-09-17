////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestBuilder
{
    using Fx.OdataPocRoot.GraphContext;
    using System;
    using System.Linq.Expressions;

    public static class GetCollectionRequestBuilderExtensions
    {
        public static IGetCollectionRequestBuilder<TInstance> Expand<TInstance, TProperty>(
            this IGetCollectionRequestBuilder<TInstance> builder,
            Expression<Func<TInstance, TProperty>> query)
        {
            throw new NotImplementedException("tODO implement linq-to-odata expand visitor");
        }

        public static IGetCollectionRequestBuilder<TInstance> Filter<TInstance>(
            this IGetCollectionRequestBuilder<TInstance> builder,
            Expression<Func<TInstance, bool>> query)
        {
            var filter = LinqToOdata.Filter(query);
            var newBuilder = builder.Builder.Filter(filter);
            return builder.Unit(newBuilder);
        }

        public static IGetCollectionRequestBuilder<TInstance> Select<TInstance, TProperty>(
            this IGetCollectionRequestBuilder<TInstance> builder,
            Expression<Func<TInstance, TProperty>> query)
        {
            var select = LinqToOdata.Select(query);
            var newBuilder = builder.Builder.Select(select);
            return builder.Unit(newBuilder);
        }

        public static OdataRequest<TInstance>.GetCollection Request<TInstance>(
            this IGetCollectionRequestBuilder<TInstance> builder)
        {
            //// TODO any other logic needed here?
            return new OdataRequest<TInstance>.GetCollection(builder.Builder.Request);
        }
    }
}
