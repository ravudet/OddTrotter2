namespace Fx.QueryContextOption1
{
    using System;

    public interface IQueryResultNode<out TValue, out TError>
    {
        TResult Visit<TResult, TContext>(Func<IElement<TValue, TError>, TContext, TResult> elementAccept, Func<ITerminal<TError>, TContext, TResult> terminalAccept);
    }

    public interface IElement<out TValue, out TError>
    {
        TValue Value { get; }

        IQueryResultNode<TValue, TError> Next();
    }

    public interface ITerminal<out TError>
    {
        TResult Visit<TResult, TContext>(Func<IError<TError>, TContext, TResult> errorAccept, Func<IEmpty, TContext, TResult> emptyAccept);
    }

    public interface IError<out TError>
    {
        TError Value { get; }
    }

    public interface IEmpty
    {
    }

    public abstract class QueryResultNode<TValue, TError>
    {
        private QueryResultNode()
        {
            //// TODO should the nodes be eithers?
        }

        protected abstract TResult Dispatch<TResult, TContext>(QueryResultNode<TValue, TError>.Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(QueryResultNode<TValue, TError> node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(QueryResultNode<TValue, TError>.Element node, TContext context);
            protected internal abstract TResult Accept(QueryResultNode<TValue, TError> .Terminal node, TContext context);
        }

        public abstract class Element : QueryResultNode<TValue, TError>
        {
            public abstract TValue Value { get; }

            public abstract QueryResultNode<TValue, TError> Next();

            protected override TResult Dispatch<TResult, TContext>(QueryResultNode<TValue, TError>.Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }
        }

        public abstract class Terminal : QueryResultNode<TValue, TError>
        {
            private Terminal()
            {
                //// TODO should this be an either?
            }

            protected sealed override TResult Dispatch<TResult, TContext>(QueryResultNode<TValue, TError>.Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }

            protected abstract TResult Dispatch<TResult, TContext>(QueryResultNode<TValue, TError>.Terminal.Visitor<TResult, TContext> visitor, TContext context);

            public new abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(QueryResultNode<TValue, TError>.Terminal node, TContext context)
                {
                    return node.Dispatch(this, context);
                }

                protected internal abstract TResult Accept(QueryResultNode<TValue, TError>.Terminal.Error node, TContext context);
                protected internal abstract TResult Accept(QueryResultNode<TValue, TError>.Terminal.Empty node, TContext context);
            }

            public sealed class Error : Terminal //// TODO do you like this name?
            {
                public Error(TError value)
                {
                    Value = value;
                }

                public TError Value { get; }

                protected override TResult Dispatch<TResult, TContext>(QueryResultNode<TValue, TError>.Terminal.Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }

            public sealed class Empty : Terminal //// TODO do you like this name?
            {
                private Empty()
                {
                }

                public static QueryResultNode<TValue, TError>.Terminal.Empty Instance { get; } = new QueryResultNode<TValue, TError>.Terminal.Empty();

                protected override TResult Dispatch<TResult, TContext>(QueryResultNode<TValue, TError>.Terminal.Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }
        }
    }
}
