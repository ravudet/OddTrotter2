namespace Fx.QueryContextOption1
{
    using System;

    public interface ITerminal<out TValue, out TError> : IQueryResultNode<TValue, TError>
    {
        TResult Visit<TResult, TContext>(Func<IError<TValue, TError>, TContext, TResult> errorAccept, Func<IEmpty<TValue, TError>, TContext, TResult> emptyAccept, TContext context);
    }
}
