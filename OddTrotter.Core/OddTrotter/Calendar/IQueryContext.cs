////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace OddTrotter.Calendar
{
    using OddTrotter.TodoList;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using static System.Net.Mime.MediaTypeNames;

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
                return WhereVisitor<TValue, TError>.Instance.Visit(this.queryResult, this.predicate);
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

        public static QueryResult<TValue, TError> ToQueryResult<TValue, TError>(this IEnumerable<TValue> enumerable)
        {
            using (var enumerator = enumerable.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return new QueryResult<TValue, TError>.Final();
                }

                //// TODO is this actually lazy?
                return new QueryResult<TValue, TError>.Element(enumerator.Current, () => ToQueryResult<TValue, TError>(enumerator));
            }
        }

        private static QueryResult<TValue, TError> ToQueryResult<TValue, TError>(IEnumerator<TValue> enumerator)
        {
            if (!enumerator.MoveNext())
            {
                return new QueryResult<TValue, TError>.Final();
            }

            return new QueryResult<TValue, TError>.Element(enumerator.Current, () => ToQueryResult<TValue, TError>(enumerator));
        }

        public static QueryResult<TValue, TError> Concat<TValue, TError>(this QueryResult<TValue, TError> first, QueryResult<TValue, TError> second)
        {
            if (first is QueryResult<TValue, TError>.Final)
            {
                return second;
            }
            else if (first is QueryResult<TValue, TError>.Partial partial)
            {
                return first;
                //// TODO
            }
            else if (first is QueryResult<TValue, TError>.Element element)
            {
                return first;
                //// TODO
                ////return new QueryResult<TValue, TError>.Element(element.Value, element.Next.ContinueWith(_ => _.Result.Concat(second)));
            }
            else
            {
                throw new Exception("TODO use visitor");
            }
        }

        public static async Task<QueryResult<TValueEnd, TError>> Select<TValueStart, TValueEnd, TError>(this QueryResult<TValueStart, TError> queryResult, Func<TValueStart, TValueEnd> selector)
        {
            await Task.Delay(1).ConfigureAwait(false); //// TODO remove this

            //// TODO is it a good idea for this to be async?
            if (queryResult is QueryResult<TValueStart, TError>.Final)
            {
                return new QueryResult<TValueEnd, TError>.Final();
            }
            else if (queryResult is QueryResult<TValueStart, TError>.Partial partial)
            {
                return new QueryResult<TValueEnd, TError>.Partial(partial.Error);
            }
            else if (queryResult is QueryResult<TValueStart, TError>.Element element)
            {
                return new QueryResult<TValueEnd, TError>.Final();
                //// TODO
                /*return new QueryResult<TValueEnd, TError>.Element(
                    selector(element.Value),
                    (await element.Next.ConfigureAwait(false)).Select(selector));*/
            }
            else
            {
                throw new Exception("TODO use visitor");
            }
        }
    }
}
