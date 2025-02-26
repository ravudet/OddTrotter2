namespace Fx.QueryContext
{
    using Fx.Either;

    public sealed class QueryResultNode<TValue, TError> : IQueryResultNode<TValue, TError>
    {
        private readonly IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>> node;

        public QueryResultNode(IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>> node)
        {
            this.node = node;
        }

        public TResult Apply<TResult, TContext>(System.Func<IElement<TValue, TError>, TContext, TResult> leftMap, System.Func<IEither<IError<TError>, IEmpty>, TContext, TResult> rightMap, TContext context)
        {
            return node.Apply(leftMap, rightMap, context);
        }
    }
}
