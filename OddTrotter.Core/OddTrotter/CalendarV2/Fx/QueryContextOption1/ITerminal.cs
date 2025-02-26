namespace Fx.QueryContextOption1
{
    using System;

    public interface ITerminal<out TValue, out TError> : IQueryResultNode<TValue, TError>
    {
        TResult Visit<TResult, TContext>(Func<IError<TValue, TError>, TContext, TResult> errorAccept, Func<IEmpty<TValue, TError>, TContext, TResult> emptyAccept, TContext context);

        TResult IQueryResultNode<TValue, TError>.Visit<TResult, TContext>(Func<IElement<TValue, TError>, TContext, TResult> elementAccept, Func<ITerminal<TValue, TError>, TContext, TResult> terminalAccept, TContext context)
        {
            //// TODO is it ok that this can be overriden?
            return terminalAccept(this, context);
        }
    }
}
