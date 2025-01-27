namespace CalendarV2.Fx.QueryContext
{
    using global::System;
    using global::System.Linq;

    using CalendarV2.System.Linq;
    using CalendarV2.Fx.Either;

    public static class QueryResultExtensions
    {
        public static Either<EnumerableExtensions.EitherFirstOrDefaultResult<TElement, TDefault>, TError> FirstOrDefault<TElement, TError, TDefault>(
            this QueryResult<TElement, TError> queryResult,
            TDefault @default)
        {
            return FirstOrDefaultVisitor<TElement, TError, TDefault>.Instance.Visit(queryResult, @default);
        }

        private sealed class FirstOrDefaultVisitor<TElement, TError, TDefault> : QueryResult<TElement, TError>.Visitor<Either<EnumerableExtensions.EitherFirstOrDefaultResult<TElement, TDefault>, TError>, TDefault>
        {
            private FirstOrDefaultVisitor()
            {
            }

            public static FirstOrDefaultVisitor<TElement, TError, TDefault> Instance { get; } = new FirstOrDefaultVisitor<TElement, TError, TDefault>();

            public override Either<EnumerableExtensions.EitherFirstOrDefaultResult<TElement, TDefault>, TError> Accept(QueryResult<TElement, TError>.Full node, TDefault context)
            {
                return new Either<EnumerableExtensions.EitherFirstOrDefaultResult<TElement, TDefault>, TError>.Left(node.Values.EitherFirstOrDefault(context));
            }

            public override Either<EnumerableExtensions.EitherFirstOrDefaultResult<TElement, TDefault>, TError> Accept(QueryResult<TElement, TError>.Partial node, TDefault context)
            {
                var firstOrError = node.Values.EitherFirstOrDefault(node.Error);
                return firstOrError.Visit(
                    (left, context) => 
                        new Either
                            <
                                EnumerableExtensions.EitherFirstOrDefaultResult
                                    <
                                        TElement, 
                                        TDefault
                                    >, 
                                TError
                            >
                            .Left(
                                new EnumerableExtensions.EitherFirstOrDefaultResult
                                    <
                                        TElement, 
                                        TDefault
                                    >(
                                        new Either
                                            <
                                                TElement, 
                                                TDefault
                                            >
                                            .Left(
                                                left)))
                            .AsBase(),
                    (right, context) => 
                        new Either
                            <
                                EnumerableExtensions.EitherFirstOrDefaultResult
                                    <
                                        TElement, 
                                        TDefault
                                    >, 
                                TError
                            >
                            .Right(
                                right)
                            .AsBase(),
                    new CalendarV2.System.Void());
            }
        }

        public static QueryResult<TValue, TError> Where<TValue, TError>(
            this QueryResult<TValue, TError> queryResult,
            Func<TValue, bool> predicate)
        {
            return WhereVisitor<TValue, TError>.Instance.Visit(queryResult, predicate);
        }

        private sealed class WhereVisitor<TValue, TError> : QueryResult<TValue, TError>.Visitor<QueryResult<TValue, TError>, Func<TValue, bool>>
        {
            private WhereVisitor()
            {
            }

            public static WhereVisitor<TValue, TError> Instance { get; } = new WhereVisitor<TValue, TError>();

            public override QueryResult<TValue, TError> Accept(QueryResult<TValue, TError>.Full node, Func<TValue, bool> context)
            {
                return new QueryResult<TValue, TError>.Full(node.Values.Where(context));
            }

            public override QueryResult<TValue, TError> Accept(QueryResult<TValue, TError>.Partial node, Func<TValue, bool> context)
            {
                return new QueryResult<TValue, TError>.Partial(node.Values.Where(context), node.Error);
            }
        }
    }
}
