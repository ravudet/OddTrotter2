using Fx.Either;

namespace Fx.QueryContextOption1.EitherNodesV2
{
    /// <summary>
    /// TODO covariance and contravariance if possible
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TError"></typeparam>
    public interface IQueryResult<out TValue, out TError>
    {
        IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>> Nodes { get; }
    }
}
