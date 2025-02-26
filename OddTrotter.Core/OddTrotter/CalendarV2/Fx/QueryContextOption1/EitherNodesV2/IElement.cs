using System;

using Fx.Either;

namespace Fx.QueryContextOption1.EitherNodesV2
{
    public interface IElement<out TValue, out TError>
    {
        TValue Value { get; }

        IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>> Next();
    }
}
