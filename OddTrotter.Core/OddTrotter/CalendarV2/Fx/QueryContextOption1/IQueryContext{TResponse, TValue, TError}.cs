namespace Fx.QueryContextOption1
{
    using Stash;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// TODO covariance and contravariance
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TError"></typeparam>
    public interface IQueryContext<TResponse, TValue, TError>
    {
        Task<IQueryResult<TResponse, TError>> Evaluate();
    }
}
