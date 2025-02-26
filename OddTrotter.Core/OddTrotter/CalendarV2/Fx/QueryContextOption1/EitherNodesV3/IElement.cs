using System;

using Fx.Either;

namespace Fx.QueryContextOption1.EitherNodesV3
{
    public interface IElement<out TValue, out TError>
    {
        TValue Value { get; }

        IQueryResultNode<TValue, TError> Next();
    }
}
