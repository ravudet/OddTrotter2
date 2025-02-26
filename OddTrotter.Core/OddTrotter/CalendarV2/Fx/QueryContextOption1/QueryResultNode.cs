namespace Fx.QueryContextOption1
{
    using Fx.Either;
    using System;

    //// TODO can you get rid of `iempty` and use `nothing` instead? (or perhaps just have no parameter for that case) //// TODO maybe eithers just avoids this entirely?

    public abstract class QueryResultNode<TValue, TError> : IQueryResultNode<TValue, TError>, EitherNodes.IQueryResultNode<TValue, TError>
    {
        private QueryResultNode()
        {
            //// TODO should the nodes be eithers?
        }

        public TResult Visit<TResult, TContext>(Func<IElement<TValue, TError>, TContext, TResult> elementAccept, Func<ITerminal<TValue, TError>, TContext, TResult> terminalAccept, TContext context)
        {
            return new DelegateVisitor<TResult, TContext>(elementAccept, terminalAccept).Visit(this, context);
        }

        private sealed class DelegateVisitor<TResult, TContext> : Visitor<TResult, TContext>
        {
            private readonly Func<Element, TContext, TResult> elementAccept;
            private readonly Func<Terminal, TContext, TResult> terminalAccept;

            public DelegateVisitor(Func<Element, TContext, TResult> elementAccept, Func<Terminal, TContext, TResult> terminalAccept)
            {
                this.elementAccept = elementAccept;
                this.terminalAccept = terminalAccept;
            }

            protected internal override TResult Accept(Element node, TContext context)
            {
                return this.elementAccept(node, context);
            }

            protected internal override TResult Accept(Terminal node, TContext context)
            {
                return this.terminalAccept(node, context);
            }
        }

        protected abstract TResult Dispatch<TResult, TContext>(QueryResultNode<TValue, TError>.Visitor<TResult, TContext> visitor, TContext context);

        public TResult Apply<TResult, TContext>(Func<EitherNodes.IElement<TValue, TError>, TContext, TResult> leftMap, Func<EitherNodes.ITerminal<TError>, TContext, TResult> rightMap, TContext context)
        {
            throw new NotImplementedException();
        }

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(QueryResultNode<TValue, TError> node, TContext context)
            {
                return node.Dispatch(this, context);
            }

            protected internal abstract TResult Accept(QueryResultNode<TValue, TError>.Element node, TContext context);
            protected internal abstract TResult Accept(QueryResultNode<TValue, TError> .Terminal node, TContext context);
        }

        public abstract class Element : QueryResultNode<TValue, TError>, IElement<TValue, TError>
        {
            public abstract TValue Value { get; }

            public abstract QueryResultNode<TValue, TError> Next();

            protected override TResult Dispatch<TResult, TContext>(QueryResultNode<TValue, TError>.Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Accept(this, context);
            }

            IQueryResultNode<TValue, TError> IElement<TValue, TError>.Next()
            {
                return this.Next();
            }
        }

        public abstract class Terminal : QueryResultNode<TValue, TError>, ITerminal<TValue, TError>
        {
            private Terminal()
            {
                //// TODO should this be an either?
            }

            public TResult Visit<TResult, TContext>(Func<IError<TValue, TError>, TContext, TResult> errorAccept, Func<IEmpty<TValue, TError>, TContext, TResult> emptyAccept, TContext context)
            {
                return new DelegateVisitor<TResult, TContext>(errorAccept, emptyAccept).Visit(this, context);
            }

            private new sealed class DelegateVisitor<TResult, TContext> : Visitor<TResult, TContext>
            {
                private readonly Func<Error, TContext, TResult> errorAccept;
                private readonly Func<Empty, TContext, TResult> emptyAccept;

                public DelegateVisitor(Func<Error, TContext, TResult> errorAccept, Func<Empty, TContext, TResult> emptyAccept)
                {
                    this.errorAccept = errorAccept;
                    this.emptyAccept = emptyAccept;
                }

                protected internal override TResult Accept(Error node, TContext context)
                {
                    return this.errorAccept(node, context);
                }

                protected internal override TResult Accept(Empty node, TContext context)
                {
                    return this.emptyAccept(node, context);
                }
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

            public sealed class Error : Terminal, IError<TValue, TError> //// TODO do you like this name?
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

            public sealed class Empty : Terminal, IEmpty<TValue, TError> //// TODO do you like this name?
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
