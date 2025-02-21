namespace Fx.QueryContextOption1
{
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

    public interface IQueryContext<TValue, TError> : IQueryContext<TValue, TValue, TError> //// TODO is this overload really helpful? maybe, but do you want to set a precedent that you need to do that for the mixins too? or maybe mixins are "advanced" enough that you should expect the dev to be able to handle it
    {
    }
}
