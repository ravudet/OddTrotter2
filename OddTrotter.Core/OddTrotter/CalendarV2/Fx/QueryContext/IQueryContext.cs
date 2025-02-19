namespace Fx.QueryContext
{
    using System.Collections.Generic;

    /// <summary>
    /// TODO covariance and contravariance
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TError"></typeparam>
    public interface IQueryContext<TValue, TError>
    {
        IAsyncEnumerable<QueryResultNode<TValue, TError>> Evaluate();
    }
}
