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

        public sealed class Element : QueryResult<TValue, TError>
        {
            public Element(TValue value, Func<QueryResult<TValue, TError>> next)
            {
                this.Value = value;
                this.Next = next;
            }

            public TValue Value { get; }

            public Func<QueryResult<TValue, TError>> Next { get; } //// TODO how inefficient is it to use Task here when the element is already known?

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
        public static QueryResult<TValue, TError> Where<TValue, TError>(this QueryResult<TValue, TError> queryResult, Func<TValue, bool> predicate)
        {
            if (queryResult is QueryResult<TValue, TError>.Final)
            {
                return queryResult;
            }
            else if (queryResult is QueryResult<TValue, TError>.Partial partial)
            {
                return queryResult;
            }
            else if (queryResult is QueryResult<TValue, TError>.Element element)
            {
                if (predicate(element.Value))
                {
                    return new QueryResult<TValue, TError>.Element(element.Value, () => Where(element.Next(), predicate));
                }
                else
                {
                    return Where(element.Next(), predicate);
                }
            }
            else
            {
                throw new Exception("TODO use visitor");
            }
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
