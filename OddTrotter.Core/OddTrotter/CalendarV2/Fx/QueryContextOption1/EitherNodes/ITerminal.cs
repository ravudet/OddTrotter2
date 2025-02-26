namespace Fx.QueryContextOption1.EitherNodes
{
    using Fx.Either;
    using System;

    public interface ITerminal<out TError> : IEither<IError<TError>, IEmpty>
    {
    }
}
