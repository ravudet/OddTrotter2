namespace OddTrotter.Calendar
{
    using System;
    using System.Linq.Expressions;

    public interface IWhereQueryContextMixin<TValue, TTranslationError, TQueryError, TQueryContext> : IQueryContext<Either<TValue, TTranslationError>, TQueryError> where TQueryContext : IQueryContext<Either<TValue, TTranslationError>, TQueryError>
    {
        TQueryContext Where(Expression<Func<TValue, bool>> predicate);
    }
}
