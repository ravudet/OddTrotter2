namespace Fx.QueryContextOption1.EitherNodesV3
{
    using Fx.Either;

    public static class QueryResultNodeExtensions
    {
        public static QueryResultNode<TValue, TError> ToQueryResultNode<TValue, TError>(this IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>> node)
        {
            return new QueryResultNode<TValue, TError>(node);
        }
    }
}
