namespace Fx.QueryContextOption1.EitherNodes
{
    using Fx.Either;
    using System;

    public interface IQueryResultNode<out TValue, out TError> : IEither<IElement<TValue, TError>, ITerminal<TError>>
    {
    }
}
