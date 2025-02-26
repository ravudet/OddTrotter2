namespace Fx.QueryContext
{
    using Fx.Either;
    using Fx.QueryContextOption1;
    using System;

    public static class QueryResultNodeExtensions
    {
        public static QueryResultNode<TValue, TError> ToQueryResultNode<TValue, TError>(this IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>> node)
        {
            return new QueryResultNode<TValue, TError>(node);
        }

        public static IQueryResultNode<TValue, TError> Where<TValue, TError>(this IQueryResultNode<TValue, TError> source, Func<TValue, bool> predicate)
        {
            return source
                .SelectLeft(
                    element => Either2
                        .Create(
                            element,
                            _ => predicate(_.Value),
                            _ => new WhereElement<TValue, TError>(_.Value, _.Next(), predicate),
                            _ => _.Next().Where(predicate))
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
                Value = value;
                this.next = next;
                this.predicate = predicate;
            }

            public TValue Value { get; }

            public IQueryResultNode<TValue, TError> Next()
            {
                return next.Where(predicate);
            }
        }

        public static IQueryResultNode<TValueResult, TError> Select<TValueSource, TError, TValueResult>(this IQueryResultNode<TValueSource, TError> source, Func<TValueSource, TValueResult> selector)
        {
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
                Value = value;
                this.next = next;
                this.selector = selector;
            }

            public TValueResult Value { get; }

            public IQueryResultNode<TValueResult, TError> Next()
            {
                return next.Select(selector);
            }
        }
    }

    public static class Either2
    {
        public static IEither<TLeft, TRight> Create<TValue, TLeft, TRight>(TValue value, Func<TValue, bool> discriminator, Func<TValue, TLeft> leftFactory, Func<TValue, TRight> rightFactory)
        {
            if (discriminator(value))
            {
                return Fx.Either.Either.Left(leftFactory(value)).Right<TRight>();
            }
            else
            {
                return Fx.Either.Either.Left<TLeft>().Right(rightFactory(value));
            }
        }
    }
}
