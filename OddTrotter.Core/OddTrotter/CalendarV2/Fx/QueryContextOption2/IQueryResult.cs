namespace Fx.QueryContextOption2
{
    using System;
    using System.Collections.Generic;

    public interface IQueryResult<out TValue, out TError>
    {
        TResult Visit<TValuesResult, TResult>(Func<IEnumerable<TValue>, TValuesResult> valuesMap, Func<TValuesResult, TError, TResult> errorAggregator, Func<TValuesResult, TResult> successMap);
    }
}
