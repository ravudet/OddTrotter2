namespace Fx.QueryContextOption1
{
    using System;

    public interface IQueryResultNode<out TValue, out TError>
    {
        TResult Visit<TResult, TContext>(Func<IElement<TValue, TError>, TContext, TResult> elementAccept, Func<ITerminal<TError>, TContext, TResult> terminalAccept, TContext context);
    }
}
