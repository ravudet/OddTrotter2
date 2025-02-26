namespace Fx.QueryContextOption1.EitherNodesV3
{
    using System;

    using Fx.Either;

    public interface IQueryResultNode<out TValue, out TError> : IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>>
    {
    }
}
