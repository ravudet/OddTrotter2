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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="predicate"/> is <see langword="null"/></exception>
        /// <exception cref="NotSupportedException">Thrown if <paramref name="predicate"/> is not supported by the underlying data store</exception>
        /// <exception cref="NotImplementedException">Thrown if support for <paramref name="predicate"/> is not yet implement; this *could* mean that <paramref name="predicate"/> is not supported by the underyling data store and such a check is not yet implemented, or it could mean that <paramref name="predicate"/> *is* supported by the underlying data store, but converting it to the proper query hasn't been implemented yet</exception>
        TQueryContext Where(Expression<Func<TValue, bool>> predicate);
    }
}
