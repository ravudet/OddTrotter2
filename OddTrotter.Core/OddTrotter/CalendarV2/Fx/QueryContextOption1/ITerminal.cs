namespace Fx.QueryContextOption1
{
    using System;

    public interface ITerminal<out TError>
    {
        TResult Visit<TResult, TContext>(Func<IError<TError>, TContext, TResult> errorAccept, Func<IEmpty, TContext, TResult> emptyAccept, TContext context);
    }
}
