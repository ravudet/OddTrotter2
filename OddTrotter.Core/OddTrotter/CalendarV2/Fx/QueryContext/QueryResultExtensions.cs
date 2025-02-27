/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.QueryContext
{
    using System;

    public static class QueryResultExtensions
    {
        /// <summary>
        /// placeholder
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TError"></typeparam>
        /// <param name="source"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="source"/> or <paramref name="predicate"/> is <see langword="null"/>
        /// </exception>
        public static IQueryResult<TValue, TError> Where<TValue, TError>(
            this IQueryResult<TValue, TError> source, 
            Func<TValue, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(predicate);

            return new WhereQueryResult<TValue, TError>(source, predicate);
        }

        private sealed class WhereQueryResult<TValue, TError> : IQueryResult<TValue, TError>
        {
            private readonly IQueryResult<TValue, TError> source;
            private readonly Func<TValue, bool> predicate;

            /// <summary>
            /// placeholder
            /// </summary>
            /// <param name="source"></param>
            /// <param name="predicate"></param>
            /// <exception cref="ArgumentNullException">
            /// Thrown if <paramref name="source"/> or <paramref name="predicate"/> is <see langword="null"/>
            /// </exception>
            public WhereQueryResult(IQueryResult<TValue, TError> source, Func<TValue, bool> predicate)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(predicate);

                this.source = source;
                this.predicate = predicate;
            }

            /// <inheritdoc/>
            public IQueryResultNode<TValue, TError> Nodes
            {
                get
                {
                    return this.source.Nodes.Where(predicate);
                }
            }
        }

        /// <summary>
        /// placeholder
        /// </summary>
        /// <typeparam name="TValueSource"></typeparam>
        /// <typeparam name="TError"></typeparam>
        /// <typeparam name="TValueResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="source"/> or <paramref name="selector"/> is <see langword="null"/>
        /// </exception>
        public static IQueryResult<TValueResult, TError> Select<TValueSource, TError, TValueResult>(
            this IQueryResult<TValueSource, TError> source, 
            Func<TValueSource, TValueResult> selector)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);

            return new SelectQueryResult<TValueSource, TError, TValueResult>(source, selector);
        }

        private sealed class SelectQueryResult<TValueSource, TError, TValueResult> : IQueryResult<TValueResult, TError>
        {
            private readonly IQueryResult<TValueSource, TError> source;
            private readonly Func<TValueSource, TValueResult> selector;

            /// <summary>
            /// placeholder
            /// </summary>
            /// <param name="source"></param>
            /// <param name="selector"></param>
            /// <exception cref="ArgumentNullException">
            /// Thrown if <paramref name="source"/> or <paramref name="selector"/> is <see langword="null"/>
            /// </exception>
            public SelectQueryResult(IQueryResult<TValueSource, TError> source, Func<TValueSource, TValueResult> selector)
            {
                ArgumentNullException.ThrowIfNull(source);
                ArgumentNullException.ThrowIfNull(selector);

                this.source = source;
                this.selector = selector;
            }

            /// <inheritdoc/>
            public IQueryResultNode<TValueResult, TError> Nodes
            {
                get
                {
                    return this.source.Nodes.Select(selector);
                }
            }
        }
    }
}
