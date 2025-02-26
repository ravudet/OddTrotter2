using Fx.Either;
using Stash;
using System;

namespace Fx.QueryContextOption1.EitherNodesV2
{
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

            public IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>> Nodes
            {
                get
                {
                    return Where(this.source.Nodes, this.predicate);
                }
            }

            private static IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>> Where(IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>> node, Func<TValue, bool> predicate)
            {
                //// TODO eithers aren't lazy, so this will fully evaluate if you use the current concrete type; but if you have a deferredeither, then i think you don't need this queryresult class
                return node
                    .SelectLeft(
                        element => Either
                            .Create(
                                element,
                                element2 => predicate(element2.Value),
                                element2 => new Element(element2.Value, element2.Next(), predicate),
                                element2 => Where(element2.Next(), predicate))
                            .SelectManyRight())
                    .SelectManyLeft();
            }

            private sealed class Element : IElement<TValue, TError>
            {
                private readonly IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>> next;
                private readonly Func<TValue, bool> predicate;

                public Element(TValue value, IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>> next, Func<TValue, bool> predicate)
                {
                    Value = value;
                    this.next = next;
                    this.predicate = predicate;
                }

                public TValue Value { get; }

                public IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>> Next()
                {
                    return WhereQueryResult<TValue, TError>.Where(this.next, this.predicate);
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

            public IEither<IElement<TValueResult, TError>, IEither<IError<TError>, IEmpty>> Nodes
            {
                get
                {
                    return Select(this.source.Nodes, this.selector);
                }
            }

            private static IEither<IElement<TValueResult, TError>, IEither<IError<TError>, IEmpty>> Select(IEither<IElement<TValueSource, TError>, IEither<IError<TError>, IEmpty>> node, Func<TValueSource, TValueResult> selector)
            {
                return node.SelectLeft(
                    element => new Element(selector(element.Value), element.Next(), selector));
            }

            private sealed class Element : IElement<TValueResult, TError>
            {
                private readonly IEither<IElement<TValueSource, TError>, IEither<IError<TError>, IEmpty>> next;
                private readonly Func<TValueSource, TValueResult> selector;

                public Element(TValueResult value, IEither<IElement<TValueSource, TError>, IEither<IError<TError>, IEmpty>> next, Func<TValueSource, TValueResult> selector)
                {
                    Value = value;
                    this.next = next;
                    this.selector = selector;
                }

                public TValueResult Value { get; }

                public IEither<IElement<TValueResult, TError>, IEither<IError<TError>, IEmpty>> Next()
                {
                    return SelectQueryResult<TValueSource, TError, TValueResult>.Select(this.next, this.selector);
                }
            }
        }

        private static class Either
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
}
