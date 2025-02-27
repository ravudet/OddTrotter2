/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.QueryContext
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Numerics;
    using Fx.Either;

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

        public static IEither<FirstOrDefault<TElement, TDefault>, TError> FirstOrDefault<TElement, TError, TDefault>(this IQueryResult<TElement, TError> source, TDefault @default)
        {
            return source
                .Nodes
                .Apply(
                    element =>
                        Either
                            .Left(
                                new FirstOrDefault<TElement, TDefault>(
                                        Either
                                            .Left(element.Value)
                                            .Right<TDefault>()))
                            .Right<TError>(),
                    terminal =>
                        terminal
                            .Apply(
                                error =>
                                    Either
                                        .Left<FirstOrDefault<TElement, TDefault>>()
                                        .Right(error.Value),
                                empty =>
                                    Either
                                        .Left(
                                            new FirstOrDefault<TElement, TDefault>(
                                                Either.Left<TElement>().Right(@default)))
                                        .Right<TError>()));
        }

        public static IQueryResult<TValue, TErrorResult> Concat<TValue, TErrorFirst, TErrorSecond, TErrorResult>(this IQueryResult<TValue, TErrorFirst> first, IQueryResult<TValue, TErrorSecond> second, Func<TErrorFirst?, TErrorSecond?, TErrorResult> errorAggregator)
        {
            return new ConcatQueryResult<TValue, TErrorFirst, TErrorSecond, TErrorResult>(first, second, errorAggregator);
        }

        private sealed class ConcatQueryResult<TValue, TErrorFirst, TErrorSecond, TErrorResult> : IQueryResult<TValue, TErrorResult>
        {
            private readonly IQueryResult<TValue, TErrorFirst> first;
            private readonly IQueryResult<TValue, TErrorSecond> second;
            private readonly Func<TErrorFirst?, TErrorSecond?, TErrorResult> errorAggregator;

            public ConcatQueryResult(IQueryResult<TValue, TErrorFirst> first, IQueryResult<TValue, TErrorSecond> second, Func<TErrorFirst?, TErrorSecond?, TErrorResult> errorAggregator)
            {
                this.first = first;
                this.second = second;
                this.errorAggregator = errorAggregator;
            }

            public IQueryResultNode<TValue, TErrorResult> Nodes
            {
                get
                {
                    return first.Nodes.Concat(second.Nodes, this.errorAggregator);
                }
            }
        }

        public static IQueryResult<TValue, TError> DistinctBy<TValue, TError, TKey>(this IQueryResult<TValue, TError> source, Func<TValue, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            return new DistinctByResult<TValue, TError, TKey>(source, keySelector, comparer);
        }

        private sealed class DistinctByResult<TValue, TError, TKey> : IQueryResult<TValue, TError>
        {
            private readonly IQueryResult<TValue, TError> source;
            private readonly Func<TValue, TKey> keySelector;
            private readonly IEqualityComparer<TKey> comparer;

            public DistinctByResult(IQueryResult<TValue, TError> source, Func<TValue, TKey> keySelector, IEqualityComparer<TKey> comparer)
            {
                this.source = source;
                this.keySelector = keySelector;
                this.comparer = comparer;
            }

            public IQueryResultNode<TValue, TError> Nodes
            {
                get
                {

                }
            }
        }
    }
}
