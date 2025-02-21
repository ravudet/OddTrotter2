namespace Fx.QueryContextOption1
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
        Task<IQueryResult<TValue, TError>> Evaluate();
    }
}
