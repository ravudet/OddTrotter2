namespace Fx.QueryContext
{
    public abstract class QueryResultNode<TValue, TError>
    {
        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(QueryResultNode<TValue, TError> node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(Value node, TContext context);
            protected internal abstract TResult Accept(Error node, TContext context);
        }

        public abstract class Value : QueryResultNode<TValue, TError>
        {
            public abstract TValue TheValue { get; } //// TODO better name

            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }

            public abstract QueryResultNode<TValue, TError> Next();
        }

        public sealed class Error : QueryResultNode<TValue, TError>
        {
            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }
    }
}
