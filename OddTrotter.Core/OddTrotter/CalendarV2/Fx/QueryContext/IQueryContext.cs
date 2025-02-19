namespace Fx.QueryContext
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO covariance and contravariance
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TError"></typeparam>
    public interface IQueryContext<TValue, TError>
    {
        Task<QueryResultNode<TValue, TError>> Evaluate();
    }
}
