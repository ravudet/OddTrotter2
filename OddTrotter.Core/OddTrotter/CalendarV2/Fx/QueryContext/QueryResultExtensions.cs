namespace CalendarV2.Fx.QueryContext
{
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;

    using CalendarV2.System.Linq;
    using CalendarV2.Fx.Either;
    using global::Fx.Either;

    /// <summary>
    /// TODO use `ieither` instead of `either`
    /// </summary>
    public static class QueryResultExtensions
    {
        public static 
            IEither
                <
                    EnumerableExtensions.FirstOrDefault
                        <
                            TElement, 
                            TDefault
                        >,
                    TError
                > 
            FirstOrDefault<TElement, TError, TDefault>(
                this QueryResult<TElement, TError> queryResult,
                TDefault @default)
        {
            return FirstOrDefaultVisitor<TElement, TError, TDefault>.Instance.Visit(queryResult, @default);
        }

        private sealed class FirstOrDefaultVisitor<TElement, TError, TDefault> : 
            QueryResult
                <
                    TElement, 
                    TError
                >
            .Visitor
                <
                    Either
                        <
                            EnumerableExtensions.FirstOrDefault
                                <
                                    TElement, 
                                    TDefault
                                >, 
                            TError
                        >, 
                    TDefault
                >
        {
            private FirstOrDefaultVisitor()
            {
            }

            public static FirstOrDefaultVisitor<TElement, TError, TDefault> Instance { get; } = 
                new FirstOrDefaultVisitor<TElement, TError, TDefault>();

            public override Either<EnumerableExtensions.FirstOrDefault<TElement, TDefault>, TError> Accept(
                QueryResult<TElement, TError>.Full node, in TDefault context)
            {
                return new Either<EnumerableExtensions.FirstOrDefault<TElement, TDefault>, TError>.Left(
                    node.Values.EitherFirstOrDefault(context));
            }

            public override Either<EnumerableExtensions.FirstOrDefault<TElement, TDefault>, TError> Accept(
                QueryResult<TElement, TError>.Partial node, in TDefault context)
            {
                var firstOrError = node.Values.EitherFirstOrDefault(node.Error);
                return firstOrError.Apply(
                    (left, context) => 
                        Either
                            .Left(
                                new EnumerableExtensions.FirstOrDefault
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
                            .Right<TError>(),
                    (right, context) => 
                        Either
                            .Left
                                <
                                    EnumerableExtensions.FirstOrDefault
                                        <
                                            TElement,
                                            TDefault
                                        >
                                >()
                            .Right(
                                right),
                    new Nothing());
            }
        }

        public static QueryResult<TValue, TError> Where<TValue, TError>(
            this QueryResult<TValue, TError> queryResult,
            Func<TValue, bool> predicate)
        {
            return WhereVisitor<TValue, TError>.Instance.Visit(queryResult, predicate);
        }

        private sealed class WhereVisitor<TValue, TError> : 
            QueryResult<TValue, TError>.Visitor<QueryResult<TValue, TError>, Func<TValue, bool>>
        {
            private WhereVisitor()
            {
            }

            public static WhereVisitor<TValue, TError> Instance { get; } = new WhereVisitor<TValue, TError>();

            public override QueryResult<TValue, TError> Accept(
                QueryResult<TValue, TError>.Full node, 
                in Func<TValue, bool> context)
            {
                return new QueryResult<TValue, TError>.Full(node.Values.Where(context));
            }

            public override QueryResult<TValue, TError> Accept(
                QueryResult<TValue, TError>.Partial node, 
                in Func<TValue, bool> context)
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
            var concatContext = new ConcatContext<TValue, TErrorFirst, TErrorSecond, TErrorResult>(
                second,
                errorFirstSelector,
                errorSecondSelector,
                errorAggregator);
            return ConcatVisitor<TValue, TErrorFirst, TErrorSecond, TErrorResult>.Instance.Visit(first, concatContext);
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

        private sealed class ConcatVisitor<TValue, TErrorFirst, TErrorSecond, TErrorResult> : 
            QueryResult
                <
                    TValue,
                    TErrorFirst
                >
            .Visitor
                <
                    QueryResult
                        <
                            TValue, 
                            TErrorResult
                        >, 
                    ConcatContext
                        <
                            TValue,
                            TErrorFirst, 
                            TErrorSecond, 
                            TErrorResult
                        >
                >
        {
            private ConcatVisitor()
            {
            }

            public static ConcatVisitor<TValue, TErrorFirst, TErrorSecond, TErrorResult> Instance { get; } = 
                new ConcatVisitor<TValue, TErrorFirst, TErrorSecond, TErrorResult>();

            public override QueryResult<TValue, TErrorResult> Accept(
                QueryResult<TValue, TErrorFirst>.Full node, 
                in ConcatContext<TValue, TErrorFirst, TErrorSecond, TErrorResult> context)
            {
                var fullSecondContext = new FullSecondContext(
                    node,
                    context.ErrorSecondSelector);
                return FullSecondVisitor.Instance.Visit(context.Second, fullSecondContext);
            }

            private struct FullSecondContext
            {
                public FullSecondContext(
                    QueryResult<TValue, TErrorFirst>.Full first, 
                    Func<TErrorSecond, TErrorResult> errorSecondSelector)
                {
                    this.First = first;
                    this.ErrorSecondSelector = errorSecondSelector;
                }

                public QueryResult<TValue, TErrorFirst>.Full First { get; }
                public Func<TErrorSecond, TErrorResult> ErrorSecondSelector { get; }
            }

            private sealed class FullSecondVisitor : 
                QueryResult<TValue, TErrorSecond>.Visitor<QueryResult<TValue, TErrorResult>, FullSecondContext>
            {
                private FullSecondVisitor()
                {
                }

                public static FullSecondVisitor Instance { get; } = new FullSecondVisitor();

                public override QueryResult<TValue, TErrorResult> Accept(
                    QueryResult<TValue, TErrorSecond>.Full node, 
                    in FullSecondContext context)
                {
                    return new QueryResult<TValue, TErrorResult>.Full(context.First.Values.Concat(node.Values));
                }

                public override QueryResult<TValue, TErrorResult> Accept(
                    QueryResult<TValue, TErrorSecond>.Partial node, 
                    in FullSecondContext context)
                {
                    return new QueryResult<TValue, TErrorResult>.Partial(
                        context.First.Values.Concat(node.Values),
                        context.ErrorSecondSelector(node.Error));
                }
            }

            public override QueryResult<TValue, TErrorResult> Accept(
                QueryResult<TValue, TErrorFirst>.Partial node, 
                in ConcatContext<TValue, TErrorFirst, TErrorSecond, TErrorResult> context)
            {
                var partialSecondContext = new PartialSecondContext(
                    node,
                    context.ErrorFirstSelector,
                    context.ErrorAggregator);
                return PartialSecondVisitor.Instance.Visit(context.Second, partialSecondContext);
            }

            private struct PartialSecondContext
            {
                public PartialSecondContext(
                    QueryResult<TValue, TErrorFirst>.Partial first, 
                    Func<TErrorFirst, TErrorResult> errorFirstSelector, 
                    Func<TErrorFirst, TErrorSecond, TErrorResult> errorAggregator)
                {
                    this.First = first;
                    this.ErrorFirstSelector = errorFirstSelector;
                    this.ErrorAggregator = errorAggregator;
                }

                public QueryResult<TValue, TErrorFirst>.Partial First { get; }
                public Func<TErrorFirst, TErrorResult> ErrorFirstSelector { get; }
                public Func<TErrorFirst, TErrorSecond, TErrorResult> ErrorAggregator { get; }
            }

            private sealed class PartialSecondVisitor : 
                QueryResult<TValue, TErrorSecond>.Visitor<QueryResult<TValue, TErrorResult>, PartialSecondContext>
            {
                private PartialSecondVisitor()
                {
                }

                public static PartialSecondVisitor Instance { get; } = new PartialSecondVisitor();

                public override QueryResult<TValue, TErrorResult> Accept(
                    QueryResult<TValue, TErrorSecond>.Full node, 
                    in PartialSecondContext context)
                {
                    return new QueryResult<TValue, TErrorResult>.Partial(
                        context.First.Values.Concat(node.Values), 
                        context.ErrorFirstSelector(context.First.Error));
                }

                public override QueryResult<TValue, TErrorResult> Accept(
                    QueryResult<TValue, TErrorSecond>.Partial node, 
                    in PartialSecondContext context)
                {
                    return new QueryResult<TValue, TErrorResult>.Partial(
                        context.First.Values.Concat(node.Values),
                        context.ErrorAggregator(context.First.Error, node.Error));
                }
            }
        }

        public static QueryResult<TElementResult, TError> Select<TElementSource, TError, TElementResult>(
            this QueryResult<TElementSource, TError> queryResult,
            Func<TElementSource, TElementResult> selector)
        {
            return SelectVisitor<TElementSource, TError, TElementResult>.Instance.Visit(queryResult, selector);
        }

        private sealed class SelectVisitor<TElementSource, TError, TElementResult> : 
            QueryResult
                <
                    TElementSource, 
                    TError
                >
            .Visitor
                <
                    QueryResult
                        <
                            TElementResult,
                            TError
                        >, 
                    Func
                        <
                            TElementSource, 
                            TElementResult
                        >
                >
        {
            private SelectVisitor()
            {
            }

            public static SelectVisitor<TElementSource, TError, TElementResult> Instance { get; } = 
                new SelectVisitor<TElementSource, TError, TElementResult>();

            public override QueryResult<TElementResult, TError> Accept(
                QueryResult<TElementSource, TError>.Full node, 
                in Func<TElementSource, TElementResult> context)
            {
                return new QueryResult<TElementResult, TError>.Full(node.Values.Select(context));
            }

            public override QueryResult<TElementResult, TError> Accept(
                QueryResult<TElementSource, TError>.Partial node, 
                in Func<TElementSource, TElementResult> context)
            {
                return new QueryResult<TElementResult, TError>.Partial(node.Values.Select(context), node.Error);
            }
        }

        public static QueryResult<TValue, TError> DistinctBy<TValue, TError, TKey>(
            this QueryResult<TValue, TError> queryResult, 
            Func<TValue, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
        {
            var distinctByContext = new DistinctByContext<TValue, TError, TKey>(
                keySelector,
                comparer);
            return DistinctByVisitor<TValue, TError, TKey>.Instance.Visit(queryResult, distinctByContext);
        }

        private struct DistinctByContext<TValue, TError, TKey>
        {
            public DistinctByContext(Func<TValue, TKey> keySelector, IEqualityComparer<TKey> comparer)
            {
                this.KeySelector = keySelector;
                this.Comparer = comparer;
            }

            public Func<TValue, TKey> KeySelector { get; }
            public IEqualityComparer<TKey> Comparer { get; }
        }

        private sealed class DistinctByVisitor<TValue, TError, TKey> : 
            QueryResult<TValue, TError>.Visitor<QueryResult<TValue, TError>, DistinctByContext<TValue, TError, TKey>>
        {
            private DistinctByVisitor()
            {
            }

            public static DistinctByVisitor<TValue, TError, TKey> Instance { get; } = 
                new DistinctByVisitor<TValue, TError, TKey>();

            public override QueryResult<TValue, TError> Accept(
                QueryResult<TValue, TError>.Full node, 
                in DistinctByContext<TValue, TError, TKey> context)
            {
                return new QueryResult<TValue, TError>.Full(node.Values.DistinctBy(context.KeySelector, context.Comparer));
            }

            public override QueryResult<TValue, TError> Accept(
                QueryResult<TValue, TError>.Partial node, 
                in DistinctByContext<TValue, TError, TKey> context)
            {
                //// TODO is it misleading to distinctby without the full result? i think not because distinct doesn't need the full context for it to make sense, unlike somethinglike orderby where the first element may be different if you don't have all of the elements
                return new QueryResult<TValue, TError>.Partial(
                    node.Values.DistinctBy(context.KeySelector, context.Comparer), node.Error);
            }
        }
    }
}
