﻿namespace CalendarV2.Fx.QueryContext
{
    using global::System;
    using global::System.Linq;

    public static class QueryResultExtensions
    {
        public abstract class FirstOrDefaultResult<TElement, TError, TDefault>
        {
            private FirstOrDefaultResult()
            {
            }

            public sealed class First : FirstOrDefaultResult<TElement, TError, TDefault>
            {
                public First(TElement element)
                {
                }
            }

            public sealed class Error : FirstOrDefaultResult<TElement, TError, TDefault>
            {
                public Error(TError error)
                {
                }
            }

            public sealed class Default : FirstOrDefaultResult<TElement, TError, TDefault>
            {
                public Default(TDefault @default)
                {
                }
            }
        }

        public static FirstOrDefaultResult<TElement, TError, TDefault> FirstOrDefault<TElement, TError, TDefault>(
            this QueryResult<TElement, TError> queryResult,
            TDefault @default)
        {
            //// TODO create a linq extension for this
            return FirstOrDefaultVisitor<TElement, TError, TDefault>.Instance.Visit(queryResult, @default);
        }

        private sealed class FirstOrDefaultVisitor<TElement, TError, TDefault> : 
            QueryResult<TElement, TError>
                .Visitor
                    <
                        FirstOrDefaultResult
                            <
                                TElement,
                                TError,
                                TDefault
                            >,
                        TDefault
                    >
        {
            private FirstOrDefaultVisitor()
            {
            }

            public static FirstOrDefaultVisitor<TElement, TError, TDefault> Instance { get; } = 
                new FirstOrDefaultVisitor<TElement, TError, TDefault>();

            public override FirstOrDefaultResult<TElement, TError, TDefault> Dispatch(
                QueryResult<TElement, TError>.Full node,
                TDefault context)
            {
                using (var enumerator = node.Values.GetEnumerator())
                {
                    if (!enumerator.MoveNext())
                    {
                        return new FirstOrDefaultResult<TElement, TError, TDefault>.Default(context);
                    }

                    return new FirstOrDefaultResult<TElement, TError, TDefault>.First(enumerator.Current);
                }
            }

            public override FirstOrDefaultResult<TElement, TError, TDefault> Dispatch(QueryResult<TElement, TError>.Partial node, TDefault context)
            {
                using (var enumerator = node.Values.GetEnumerator())
                {
                    if (!enumerator.MoveNext())
                    {
                        //// TODO is this ambiguous? it could be we paged and didn't get any elements before the paging error happened //// TODO i don't think that's ambiguous, it means that we should return an error since we don't have a first or the knowledge that there are no elements, because we have an error

                        return new FirstOrDefaultResult<TElement, TError, TDefault>.Error(node.Error);
                    }

                    return new FirstOrDefaultResult<TElement, TError, TDefault>.First(enumerator.Current);
                }
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

            public override QueryResult<TValue, TError> Dispatch(QueryResult<TValue, TError>.Full node, Func<TValue, bool> context)
            {
                return new QueryResult<TValue, TError>.Full(node.Values.Where(context));
            }

            public override QueryResult<TValue, TError> Dispatch(QueryResult<TValue, TError>.Partial node, Func<TValue, bool> context)
            {
                return new QueryResult<TValue, TError>.Partial(node.Values.Where(context), node.Error);
            }
        }
    }
}
