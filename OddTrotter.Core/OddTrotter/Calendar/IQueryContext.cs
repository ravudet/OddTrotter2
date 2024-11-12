////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    using OddTrotter.TodoList;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IQueryContext<TValue, TError>
    {
        Task<QueryResult<TValue, TError>> Evaluate();
    }

    public abstract class QueryResult<TValue, TError>
    {
        private QueryResult()
        {
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(QueryResult<TValue, TError> node, TContext context)
            {
                return node.Accept(this, context);
            }

            public abstract TResult Dispatch(Final node, TContext context);

            public abstract TResult Dispatch(Element node, TContext context);

            public abstract TResult Dispatch(Partial node, TContext context);
        }

        public sealed class Final : QueryResult<TValue, TError>
        {
            public Final()
            {
            }

            protected override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Dispatch(this, context);
            }
        }

        public abstract class Element : QueryResult<TValue, TError>
        {
            public Element(TValue value)
            {
                this.Value = value;
            }

            public TValue Value { get; }

            public abstract QueryResult<TValue, TError> Next();

            protected override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Dispatch(this, context);
            }
        }

        public sealed class Partial : QueryResult<TValue, TError>
        {
            public Partial(TError error)
            {
                this.Error = error;
            }

            public TError Error { get; }

            protected override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Dispatch(this, context);
            }
        }
    }

    public static class QueryResultExtensions
    {
        private sealed class WhereResult<TValue, TError> : QueryResult<TValue, TError>.Element
        {
            private readonly Element queryResult;
            private readonly Func<TValue, bool> predicate;

            public WhereResult(QueryResult<TValue, TError>.Element queryResult, Func<TValue, bool> predicate)
                : base(queryResult.Value)
            {
                this.queryResult = queryResult;
                this.predicate = predicate;
            }

            public sealed override QueryResult<TValue, TError> Next()
            {
                return WhereVisitor<TValue, TError>.Instance.Visit(this.queryResult.Next(), this.predicate);
            }
        }

        private sealed class WhereVisitor<TValue, TError> : QueryResult<TValue, TError>.Visitor<QueryResult<TValue, TError>, Func<TValue, bool>>
        {
            private WhereVisitor()
            {
            }

            public static WhereVisitor<TValue, TError> Instance { get; } = new WhereVisitor<TValue, TError>();

            public override QueryResult<TValue, TError> Dispatch(QueryResult<TValue, TError>.Final node, Func<TValue, bool> context)
            {
                return node;
            }

            public override QueryResult<TValue, TError> Dispatch(QueryResult<TValue, TError>.Element node, Func<TValue, bool> context)
            {
                if (context(node.Value))
                {
                    return new WhereResult<TValue, TError>(node, context);
                }
                else
                {
                    return this.Visit(node.Next(), context);
                }
            }

            public override QueryResult<TValue, TError> Dispatch(QueryResult<TValue, TError>.Partial node, Func<TValue, bool> context)
            {
                return node;
            }
        }

        public static QueryResult<TValue, TError> Where<TValue, TError>(this QueryResult<TValue, TError> queryResult, Func<TValue, bool> predicate)
        {
            return WhereVisitor<TValue, TError>.Instance.Visit(queryResult, predicate);
        }

        private sealed class ToQueryResultResult<TValue, TError> : QueryResult<TValue, TError>.Element
        {
            private readonly IEnumerator<TValue> enumerator;

            public ToQueryResultResult(IEnumerator<TValue> enumerator)
                : base(enumerator.Current)
            {
                //// TODO make queryresult.final disposable?
                this.enumerator = enumerator;
            }

            public override QueryResult<TValue, TError> Next()
            {
                if (!this.enumerator.MoveNext())
                {
                    return new QueryResult<TValue, TError>.Final();
                }

                //// TODO is this ok? it might not be properly immutable this way
                return new ToQueryResultResult<TValue, TError>(this.enumerator);
            }
        }

        public static QueryResult<TValue, TError> ToQueryResult<TValue, TError>(this IEnumerable<TValue> enumerable)
        {
            IEnumerator<TValue>? enumerator = null;
            try
            {
                enumerator = enumerable.GetEnumerator();
                if (!enumerator.MoveNext())
                {
                    return new QueryResult<TValue, TError>.Final();
                }

                return new ToQueryResultResult<TValue, TError>(enumerator);
            }
            catch
            {
                enumerator?.Dispose();
                throw;
            }
        }

        private sealed class ConcatResult<TValue, TError> : QueryResult<TValue, TError>.Element
        {
            private readonly QueryResult<TValue, TError>.Element element;
            private readonly QueryResult<TValue, TError> second;

            public ConcatResult(QueryResult<TValue, TError>.Element element, QueryResult<TValue, TError> second)
                : base(element.Value)
            {
                this.element = element;
                this.second = second;
            }

            public override QueryResult<TValue, TError> Next()
            {
                return ConcatVisitor<TValue, TError>.Instance.Visit(this.element.Next(), second);
            }
        }

        private sealed class ConcatVisitor<TValue, TError> : QueryResult<TValue, TError>.Visitor<QueryResult<TValue, TError>, QueryResult<TValue, TError>>
        {
            private ConcatVisitor()
            {
            }

            public static ConcatVisitor<TValue, TError> Instance { get; } = new ConcatVisitor<TValue, TError>();

            public override QueryResult<TValue, TError> Dispatch(QueryResult<TValue, TError>.Final node, QueryResult<TValue, TError> context)
            {
                return context;
            }

            public override QueryResult<TValue, TError> Dispatch(QueryResult<TValue, TError>.Element node, QueryResult<TValue, TError> context)
            {
                return new ConcatResult<TValue, TError>(node, context);
            }

            public override QueryResult<TValue, TError> Dispatch(QueryResult<TValue, TError>.Partial node, QueryResult<TValue, TError> context)
            {
                //// TODO what if there was in error in first *and* second?
                return node;
            }
        }

        public static QueryResult<TValue, TError> Concat<TValue, TError>(this QueryResult<TValue, TError> first, QueryResult<TValue, TError> second)
        {
            //// TODO do a linq query that has more than one parameter
            return ConcatVisitor<TValue, TError>.Instance.Visit(first, second);
        }

        private sealed class SelectResult<TValueStart, TValueEnd, TError> : QueryResult<TValueEnd, TError>.Element
        {
            private readonly QueryResult<TValueStart, TError>.Element queryResult;
            private readonly Func<TValueStart, TValueEnd> selector;

            public SelectResult(QueryResult<TValueStart, TError>.Element queryResult, Func<TValueStart, TValueEnd> selector)
                : base(selector(queryResult.Value))
            {
                this.queryResult = queryResult;
                this.selector = selector;
            }

            public override QueryResult<TValueEnd, TError> Next()
            {
                return SelectVisitor<TValueStart, TValueEnd, TError>.Instance.Visit(this.queryResult.Next(), this.selector);
            }
        }

        private sealed class SelectVisitor<TValueStart, TValueEnd, TError> : QueryResult<TValueStart, TError>.Visitor<QueryResult<TValueEnd, TError>, Func<TValueStart, TValueEnd>>
        {
            private SelectVisitor()
            {
            }

            public static SelectVisitor<TValueStart, TValueEnd, TError> Instance { get; } = new SelectVisitor<TValueStart, TValueEnd, TError>();

            public override QueryResult<TValueEnd, TError> Dispatch(QueryResult<TValueStart, TError>.Final node, Func<TValueStart, TValueEnd> context)
            {
                return new QueryResult<TValueEnd, TError>.Final();
            }

            public override QueryResult<TValueEnd, TError> Dispatch(QueryResult<TValueStart, TError>.Element node, Func<TValueStart, TValueEnd> context)
            {
                throw new NotImplementedException();
            }

            public override QueryResult<TValueEnd, TError> Dispatch(QueryResult<TValueStart, TError>.Partial node, Func<TValueStart, TValueEnd> context)
            {
                return new QueryResult<TValueEnd, TError>.Partial(node.Error);
            }
        }

        public static QueryResult<TValueEnd, TError> Select<TValueStart, TValueEnd, TError>(this QueryResult<TValueStart, TError> queryResult, Func<TValueStart, TValueEnd> selector)
        {
            return SelectVisitor<TValueStart, TValueEnd, TError>.Instance.Visit(queryResult, selector);
        }
    }
}
