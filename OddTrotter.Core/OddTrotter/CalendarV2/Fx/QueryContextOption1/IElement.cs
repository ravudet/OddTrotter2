using System;

namespace Fx.QueryContextOption1
{
    public interface IElement<out TValue, out TError> : IQueryResultNode<TValue, TError>
    {
        TValue Value { get; }

        IQueryResultNode<TValue, TError> Next();

        TResult IQueryResultNode<TValue, TError>.Visit<TResult, TContext>(Func<IElement<TValue, TError>, TContext, TResult> elementAccept, Func<ITerminal<TValue, TError>, TContext, TResult> terminalAccept, TContext context)
        {
            //// TODO should this even be an implicit implementation? it doesn't really seem to do much for you
            //// TODO is it ok that this can be overriden?
            return elementAccept(this, context);
        }
    }
}
