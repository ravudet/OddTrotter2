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
            public Element(TValue value, QueryResult<TValue, TError> next)
            {
                this.Value = value;
                this.Next = next;
            }

            public TValue Value { get; }

            public QueryResult<TValue, TError> Next { get; }
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
}
