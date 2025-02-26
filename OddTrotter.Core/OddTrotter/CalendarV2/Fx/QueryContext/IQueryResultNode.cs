﻿namespace Fx.QueryContext
{
    using System;

    using Fx.Either;

    public interface IQueryResultNode<out TValue, out TError> : IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>>
    {
    }
}
