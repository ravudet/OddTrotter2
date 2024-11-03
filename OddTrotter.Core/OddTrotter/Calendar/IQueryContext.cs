////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

    public static class QueryResultDriver
    {
        public static async System.Threading.Tasks.Task DoWork(QueryResult<string, System.Exception>.Element result)
        {
            var next = await result.Next;
        }
    }
}
