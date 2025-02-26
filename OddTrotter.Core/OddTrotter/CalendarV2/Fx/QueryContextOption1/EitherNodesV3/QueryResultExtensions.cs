namespace Fx.QueryContextOption1.EitherNodesV3
{
    using System;

    public static class QueryResultExtensions
    {
        public static IQueryResult<TValue, TError> Where<TValue, TError>(this IQueryResult<TValue, TError> source, Func<TValue, bool> predicate)
        {
            return new WhereQueryResult<TValue, TError>(source, predicate);
        }

        private sealed class WhereQueryResult<TValue, TError> : IQueryResult<TValue, TError>
        {
            private readonly IQueryResult<TValue, TError> source;
            private readonly Func<TValue, bool> predicate;

            public WhereQueryResult(IQueryResult<TValue, TError> source, Func<TValue, bool> predicate)
            {
                this.source = source;
                this.predicate = predicate;
            }

            public IQueryResultNode<TValue, TError> Nodes
            {
                get
                {
                    return this.source.Nodes.Where(this.predicate);
                }
            }
        }

        public static IQueryResult<TValueResult, TError> Select<TValueSource, TError, TValueResult>(this IQueryResult<TValueSource, TError> source, Func<TValueSource, TValueResult> selector)
        {
            return new SelectQueryResult<TValueSource, TError, TValueResult>(source, selector);
        }

        private sealed class SelectQueryResult<TValueSource, TError, TValueResult> : IQueryResult<TValueResult, TError>
        {
            private readonly IQueryResult<TValueSource, TError> source;
            private readonly Func<TValueSource, TValueResult> selector;

            public SelectQueryResult(IQueryResult<TValueSource, TError> source, Func<TValueSource, TValueResult> selector)
            {
                this.source = source;
                this.selector = selector;
            }

            public IQueryResultNode<TValueResult, TError> Nodes
            {
                get
                {
                    return this.source.Nodes.Select(this.selector);
                }
            }
        }
    }
}
