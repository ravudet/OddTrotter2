namespace OddTrotter.Calendar
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// TODO better way to handle "either"s?
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TTranslationError"></typeparam>
    /// <typeparam name="TQueryError"></typeparam>
    /// <typeparam name="TQueryContext"></typeparam>
    public interface IWhereQueryContextMixin<TValue, TTranslationError, TQueryError, TQueryContext> : IQueryContext<Either<TValue, TTranslationError>, TQueryError> where TQueryContext : IQueryContext<Either<TValue, TTranslationError>, TQueryError>
    {
        TQueryContext Where(Expression<Func<TValue, bool>> predicate);
    }
}
