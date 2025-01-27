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

        public static QueryResult<TValue, TErrorResult> Concat<TValue, TErrorFirst, TErrorSecond, TErrorResult>(
            this QueryResult<TValue, TErrorFirst> first,
            QueryResult<TValue, TErrorSecond> second,
            Func<TErrorFirst, TErrorResult> errorFirstSelector,
            Func<TErrorSecond, TErrorResult> errorSecondSelector,
            Func<TErrorFirst, TErrorSecond, TErrorResult> errorAggregator)
        {
            //// TODO does this do a good job of handling each of the error cases?
            var context = new ConcatContext<TValue, TErrorFirst, TErrorSecond, TErrorResult>(
                second,
                errorFirstSelector,
                errorSecondSelector,
                errorAggregator);
            return ConcatVisitor<TValue, TErrorFirst, TErrorSecond, TErrorResult>.Instance.Visit(first, context);
        }

        private struct ConcatContext<TValue, TErrorFirst, TErrorSecond, TErrorResult>
        {
            public ConcatContext(
                QueryResult<TValue, TErrorSecond> second,
                Func<TErrorFirst, TErrorResult> errorFirstSelector,
                Func<TErrorSecond, TErrorResult> errorSecondSelector, 
                Func<TErrorFirst, TErrorSecond, TErrorResult> errorAggregator)
            {
                this.Second = second;
                this.ErrorFirstSelector = errorFirstSelector;
                this.ErrorSecondSelector = errorSecondSelector;
                this.ErrorAggregator = errorAggregator;
            }

            public QueryResult<TValue, TErrorSecond> Second { get; }
            public Func<TErrorFirst, TErrorResult> ErrorFirstSelector { get; }
            public Func<TErrorSecond, TErrorResult> ErrorSecondSelector { get; }
            public Func<TErrorFirst, TErrorSecond, TErrorResult> ErrorAggregator { get; }
        }

        private sealed class ConcatVisitor<TValue, TErrorFirst, TErrorSecond, TErrorResult> : QueryResult<TValue, TErrorFirst>.Visitor<QueryResult<TValue, TErrorResult>, ConcatContext<TValue, TErrorFirst, TErrorSecond, TErrorResult>>
        {
            private ConcatVisitor()
            {
            }

            public static ConcatVisitor<TValue, TErrorFirst, TErrorSecond, TErrorResult> Instance { get; } = new ConcatVisitor<TValue, TErrorFirst, TErrorSecond, TErrorResult>();

            public override QueryResult<TValue, TErrorResult> Accept(QueryResult<TValue, TErrorFirst>.Full node, ConcatContext<TValue, TErrorFirst, TErrorSecond, TErrorResult> context) //// TODO use `in` parameters for context?
            {
                return FullSecondVisitor.Instance.Visit(context.Second, (node, context.ErrorSecondSelector));
            }

            private sealed class FullSecondVisitor : QueryResult<TValue, TErrorSecond>.Visitor<QueryResult<TValue, TErrorResult>, (QueryResult<TValue, TErrorFirst>.Full, Func<TErrorSecond, TErrorResult>)>
            {
                private FullSecondVisitor()
                {
                }

                public static FullSecondVisitor Instance { get; } = new FullSecondVisitor();

                public override QueryResult<TValue, TErrorResult> Accept(QueryResult<TValue, TErrorSecond>.Full node, (QueryResult<TValue, TErrorFirst>.Full, Func<TErrorSecond, TErrorResult>) context)
                {
                    return new QueryResult<TValue, TErrorResult>.Full(context.Item1.Values.Concat(node.Values));
                }

                public override QueryResult<TValue, TErrorResult> Accept(QueryResult<TValue, TErrorSecond>.Partial node, (QueryResult<TValue, TErrorFirst>.Full, Func<TErrorSecond, TErrorResult>) context)
                {
                    return new QueryResult<TValue, TErrorResult>.Partial(
                        context.Item1.Values.Concat(node.Values),
                        context.Item2(node.Error));
                }
            }

            public override QueryResult<TValue, TErrorResult> Accept(QueryResult<TValue, TErrorFirst>.Partial node, ConcatContext<TValue, TErrorFirst, TErrorSecond, TErrorResult> context)
            {
                return PartialSecondVisitor.Instance.Visit(context.Second, (node, context.ErrorFirstSelector, context.ErrorAggregator));
            }

            private sealed class PartialSecondVisitor : QueryResult<TValue, TErrorSecond>.Visitor<QueryResult<TValue, TErrorResult>, (QueryResult<TValue, TErrorFirst>.Partial, Func<TErrorFirst, TErrorResult>, Func<TErrorFirst, TErrorSecond, TErrorResult>)>
            {
                private PartialSecondVisitor()
                {
                }

                public static PartialSecondVisitor Instance { get; } = new PartialSecondVisitor();

                public override QueryResult<TValue, TErrorResult> Accept(QueryResult<TValue, TErrorSecond>.Full node, (QueryResult<TValue, TErrorFirst>.Partial, Func<TErrorFirst, TErrorResult>, Func<TErrorFirst, TErrorSecond, TErrorResult>) context)
                {
                    return new QueryResult<TValue, TErrorResult>.Partial(context.Item1.Values.Concat(node.Values), context.Item2(context.Item1.Error));
                }

                public override QueryResult<TValue, TErrorResult> Accept(QueryResult<TValue, TErrorSecond>.Partial node, (QueryResult<TValue, TErrorFirst>.Partial, Func<TErrorFirst, TErrorResult>, Func<TErrorFirst, TErrorSecond, TErrorResult>) context)
                {
                    return new QueryResult<TValue, TErrorResult>.Partial(
                        context.Item1.Values.Concat(node.Values),
                        context.Item3(context.Item1.Error, node.Error));
                }
            }
        }
    }
}
