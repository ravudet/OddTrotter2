namespace Fx.QueryContextOption1.EitherNodes
{
    using System;

    using Fx.Either;

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
                    return Where(this.source.Nodes, predicate);
                    ////return SelectLeft(this.source.Nodes, this.predicate);
                }
            }

            private static IQueryResultNode<TValue, TError> Where(IQueryResultNode<TValue, TError> node, Func<TValue, bool> predicate)
            {
                var result = node.SelectLeft(element => predicate(element.Value) ? new Element(element.Value, predicate, element.Next()) : Where(element.Next(), predicate));

                var result2 = result.SelectLeft(left => AsEither(left));

                var result3 = result2.SelectManyLeft();

                //// TODO if you got eitherextensions to use concrete return types, you wouldn't need this adapter anymore (or, rather, it'd be a detail and not "in your face");
                return new EitherAdapter<TValue, TError>(result3);
            }

            private static IEither<IElement<TValue, TError>, ITerminal<TError>> AsEither(IQueryResultNode<TValue, TError> node)
            {
                return node;
            }

            private sealed class Element : IElement<TValue, TError> ////QueryResultNode<TValue, TError>.Element
            {
                private readonly Func<TValue, bool> predicate;
                private readonly IQueryResultNode<TValue, TError> next;

                public Element(TValue value, Func<TValue, bool> predicate, IQueryResultNode<TValue, TError> next)
                {
                    Value = value;
                    this.predicate = predicate;
                    this.next = next;
                }

                public TValue Value { get; }

                public TResult Apply<TResult, TContext>(Func<IElement<TValue, TError>, TContext, TResult> leftMap, Func<ITerminal<TError>, TContext, TResult> rightMap, TContext context)
                {
                    return leftMap(this, context);
                }

                public IQueryResultNode<TValue, TError> Next()
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

            public IQueryResultNode<TValueResult, TError> Nodes
            {
                get
                {
                    return Select(this.source.Nodes, this.selector);
                }
            }

            private static IQueryResultNode<TValueResult, TError> Select(IQueryResultNode<TValueSource, TError> node, Func<TValueSource, TValueResult> selector)
            {
                return new EitherAdapter<TValueResult, TError>(node.SelectLeft(element => new Element(selector(element.Value), element.Next(), selector)));
            }

            private sealed class Element : IElement<TValueResult, TError>
            {
                private readonly IQueryResultNode<TValueSource, TError> next;
                private readonly Func<TValueSource, TValueResult> selector;

                public Element(TValueResult value, IQueryResultNode<TValueSource, TError> next, Func<TValueSource, TValueResult> selector)
                {
                    Value = value;
                    this.next = next;
                    this.selector = selector;
                }

                public TValueResult Value { get; }

                public TResult Apply<TResult, TContext>(Func<IElement<TValueResult, TError>, TContext, TResult> leftMap, Func<ITerminal<TError>, TContext, TResult> rightMap, TContext context)
                {
                    return leftMap(this, context);
                }

                public IQueryResultNode<TValueResult, TError> Next()
                {
                    return SelectQueryResult<TValueSource, TError, TValueResult>.Select(this.next, this.selector);
                }
            }
        }

        private sealed class EitherAdapter<TValue, TError> : IQueryResultNode<TValue, TError>
        {
            private readonly IEither<IElement<TValue, TError>, ITerminal<TError>> either;

            public EitherAdapter(IEither<IElement<TValue, TError>, ITerminal<TError>> either)
            {
                this.either = either;
            }

            public TResult Apply<TResult, TContext>(Func<IElement<TValue, TError>, TContext, TResult> leftMap, Func<ITerminal<TError>, TContext, TResult> rightMap, TContext context)
            {
                return either.Apply(leftMap, rightMap, context);
            }
        }
    }
}
