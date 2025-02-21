namespace Fx.QueryContextOption2
{
    using System;
    using System.Linq;

    public static class QueryResultExtensions
    {
        public static IQueryResult<TValue, TError> Where<TValue, TError>(this IQueryResult<TValue, TError> queryResult, Func<TValue, bool> predicate)
        {
            return queryResult.Visit(
                values => values.Where(predicate),
                (resultingValues, error) => new QueryResult<TValue, TError>(resultingValues, error),
                resultingValues => new QueryResult<TValue, TError>(resultingValues));
        }
    }
}
