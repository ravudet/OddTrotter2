////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestBuilder
{
    using System;
    using System.Linq.Expressions;

    using Fx.OdataPocRoot.GraphContext;

    public static class GetInstanceRequestBuilderExtensions
    {
        public static IGetInstanceRequestBuilder<TInstance> Expand<TInstance, TProperty>(
            this IGetInstanceRequestBuilder<TInstance> builder,
            Expression<Func<TInstance, TProperty>> query)
        {
            throw new NotImplementedException("tODO implement linq-to-odata expand visitor");
        }

        public static IGetInstanceRequestBuilder<TInstance> Select<TInstance, TProperty>(
            this IGetInstanceRequestBuilder<TInstance> builder, 
            Expression<Func<TInstance, TProperty>> query)
        {
            //// TODO use mixins for these extensions
            
            var select = LinqToOdata.Select(query); //// TODO use a visitor for this
            var newBuilder = builder.Builder.Select(select);
            return builder.Unit(newBuilder);
        }
    }
}
