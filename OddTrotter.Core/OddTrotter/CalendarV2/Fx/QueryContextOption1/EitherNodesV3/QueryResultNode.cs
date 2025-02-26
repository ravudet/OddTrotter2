namespace Fx.QueryContextOption1.EitherNodesV3
{
    using Fx.Either;

    public sealed class QueryResultNode<TValue, TError> : IQueryResultNode<TValue, TError>
    {
        private readonly IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>> node;

        public QueryResultNode(IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>> node)
        {
            this.node = node;
        }

        public TResult Apply<TResult, TContext>(System.Func<IElement<TValue, TError>, TContext, TResult> elementMap, System.Func<IError<TError>, TContext, TResult> errorMap, System.Func<IEmpty, TContext, TResult> emptyMap, TContext context)
        {
            return this.node.Apply(elementMap, (terminal, context) => terminal.Apply(errorMap, emptyMap, context), context);
        }
    }
}
