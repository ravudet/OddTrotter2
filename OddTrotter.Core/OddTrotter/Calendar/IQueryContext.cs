////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Threading.Tasks;

namespace OddTrotter.Calendar
{
    public interface IQueryContext<TValue, TError>
    {
        QueryResult<TValue, TError> Evaluate();
    }

    public abstract class QueryResult<TValue, TError>
    {
        private QueryResult()
        {
        }

        public sealed class Final : QueryResult<TValue, TError>
        {
            public Final()
            {
            }
        }

        public sealed class Element : QueryResult<TValue, TError>
        {
            public Element(TValue value, System.Threading.Tasks.Task<QueryResult<TValue, TError>> next)
            {
                this.Value = value;
                this.Next = next;
            }

            public TValue Value { get; }

            public System.Threading.Tasks.Task<QueryResult<TValue, TError>> Next { get; } //// TODO how inefficient is it to use Task here when the element is already known?
        }

        public sealed class Partial : QueryResult<TValue, TError>
        {
            public Partial(TError error)
            {
                this.Error = error;
            }

            public TError Error { get; }
        }
    }

    public abstract class QueryResultV2<TValue, TError>
    {
        private QueryResultV2()
        {
        }

        public sealed class Final : QueryResultV2<TValue, TError>
        {
            public Final()
            {
            }
        }

        public sealed class Element : QueryResultV2<TValue, TError>
        {
            public Element(TValue value)
            {
                this.Value = value;
            }

            public TValue Value { get; }
        }

        public sealed class Partial : QueryResultV2<TValue, TError>
        {
            public Partial(TError error)
            {
                this.Error = error;
            }

            public TError Error { get; }
        }
    }

    public static class QueryResultExtensions
    {
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
                return new QueryResult<TValue, TError>.Element(element.Value, element.Next.ContinueWith(_ => _.Result.Concat(second)));
            }
        }
    }

    public static class QueryResultDriver
    {
        public static async System.Threading.Tasks.Task DoWork(QueryResult<string, System.Exception>.Element result)
        {
            var next = await result.Next;
        }
    }
}
