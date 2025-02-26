namespace Fx.QueryContextOption1.EitherNodesV3
{
    using System;

    public interface IQueryResultNode<out TValue, out TError>
    {
        TResult Apply<TResult, TContext>(Func<IElement<TValue, TError>, TContext, TResult> elementMap, Func<IError<TError>, TContext, TResult> errorMap, Func<IEmpty, TContext, TResult> emptyMap, TContext context);
    }
}
