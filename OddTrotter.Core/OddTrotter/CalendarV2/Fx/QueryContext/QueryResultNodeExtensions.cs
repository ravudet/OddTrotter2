/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.QueryContext
{
    using System;

    using Fx.Either;

    public static class QueryResultNodeExtensions
    {
        public static QueryResultNode<TValue, TError> ToQueryResultNode<TValue, TError>(this IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>> node)
        {
            ArgumentNullException.ThrowIfNull(node);

            return new QueryResultNode<TValue, TError>(node);
        }

        public static IQueryResultNode<TValue, TError> Where<TValue, TError>(this IQueryResultNode<TValue, TError> source, Func<TValue, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(predicate);

            return source
                .SelectLeft(
                    element => Either
                        .Create(
                            element,
                            element => predicate(element.Value),
                            element => new WhereElement<TValue, TError>(element.Value, element.Next(), predicate),
                            element => element.Next().Where(predicate))
                        .SelectManyRight())
                .SelectManyLeft()
                .ToQueryResultNode();
        }

        private sealed class WhereElement<TValue, TError> : IElement<TValue, TError>
        {
            private readonly IQueryResultNode<TValue, TError> next;
            private readonly Func<TValue, bool> predicate;

            public WhereElement(TValue value, IQueryResultNode<TValue, TError> next, Func<TValue, bool> predicate)
            {
                ArgumentNullException.ThrowIfNull(next);
                ArgumentNullException.ThrowIfNull(predicate);

                this.Value = value;
                this.next = next;
                this.predicate = predicate;
            }

            public TValue Value { get; }

            public IQueryResultNode<TValue, TError> Next()
            {
                return this.next.Where(predicate);
            }
        }

        public static IQueryResultNode<TValueResult, TError> Select<TValueSource, TError, TValueResult>(this IQueryResultNode<TValueSource, TError> source, Func<TValueSource, TValueResult> selector)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(selector);

            return source
                .SelectLeft(
                    element =>
                        new SelectElement<TValueSource, TError, TValueResult>(
                            selector(element.Value),
                            element.Next(),
                            selector))
                .ToQueryResultNode();
        }

        private sealed class SelectElement<TValueSource, TError, TValueResult> : IElement<TValueResult, TError>
        {
            private readonly IQueryResultNode<TValueSource, TError> next;
            private readonly Func<TValueSource, TValueResult> selector;

            public SelectElement(TValueResult value, IQueryResultNode<TValueSource, TError> next, Func<TValueSource, TValueResult> selector)
            {
                ArgumentNullException.ThrowIfNull(next);
                ArgumentNullException.ThrowIfNull(selector);

                this.Value = value;
                this.next = next;
                this.selector = selector;
            }

            public TValueResult Value { get; }

            public IQueryResultNode<TValueResult, TError> Next()
            {
                return this.next.Select(selector);
            }
        }
    }
}
