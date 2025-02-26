using System;
using Fx.Either;

namespace Fx.QueryContext
{
    public interface IElement<out TValue, out TError>
    {
        TValue Value { get; }

        IQueryResultNode<TValue, TError> Next();
    }
}
