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
                return node
                    .SelectLeft(
                        element => predicate(element.Value) ? new Element(element.Value, element.Next(), predicate) : Where(element.Next(), predicate))
                    .SelectManyLeft();
            }

            private sealed class Element : IElement<TValue, TError>, IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>>
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

                public TResult Apply<TResult, TContext>(Func<IElement<TValue, TError>, TContext, TResult> leftMap, Func<IEither<IError<TError>, IEmpty>, TContext, TResult> rightMap, TContext context)
                {
                    ////  TODO wow, you still had to write this...
                    return leftMap(this, context);
                }

                public IEither<IElement<TValue, TError>, IEither<IError<TError>, IEmpty>> Next()
                {
                    return WhereQueryResult<TValue, TError>.Where(this.next, this.predicate);
                }
            }
        }
    }
}
