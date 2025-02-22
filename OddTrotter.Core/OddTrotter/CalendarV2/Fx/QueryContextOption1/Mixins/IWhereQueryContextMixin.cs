namespace Fx.QueryContextOption1.Mixins
{
    using System;
    using System.Linq.Expressions;

    public interface IWhereQueryContextMixin<TResponse, TValue, TError, TQueryContext> : IQueryContext<TResponse, TValue, TError> where TQueryContext : IQueryContext<TResponse, TValue, TError>
    {
        TQueryContext WhereImpl(Expression<Func<TValue, bool>> predicate);
    }
}
