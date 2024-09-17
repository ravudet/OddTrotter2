namespace Fx.OdataPocRoot.GraphContext
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter;
    using System;
    using System.Linq.Expressions;

    public static partial class LinqToOdata
    {
        public static Filter Filter<TType>(Expression<Func<TType, bool>> predicate)
        {
            return new Filter(new BoolCommonExpression.First(new BooleanValue.True()));
        }
    }
}
