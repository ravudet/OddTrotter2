namespace Fx.QueryContextOption2
{
    using System;
    using System.Collections.Generic;

    public sealed class QueryResult<TValue, TError> : IQueryResult<TValue, TError>
    {
        private readonly QueryResultNode queryResultNode;

        public QueryResult(IEnumerable<TValue> values, TError error)
        {
            this.queryResultNode = QueryResultNode.Terminal.Empty.Instance;
            throw new NotImplementedException();
        }

        public QueryResult(IEnumerable<TValue> values)
        {
            this.queryResultNode = QueryResultNode.Terminal.Empty.Instance;
            throw new NotImplementedException();
        }

        public TResult Visit<TValuesResult, TResult>(Func<IEnumerable<TValue>, TValuesResult> valuesMap, Func<TValuesResult, TError, TResult> errorAggregator, Func<TValuesResult, TResult> successMap)
        {
            return VisitVisitor<TValuesResult, TResult>.Instance.Visit(this.queryResultNode, new VisitContext<TValuesResult, TResult>(valuesMap, errorAggregator, successMap));
        }

        private readonly struct VisitContext<TValuesResult, TResult>
        {
            public VisitContext(Func<IEnumerable<TValue>, TValuesResult> valuesMap, Func<TValuesResult, TError, TResult> errorAggregator, Func<TValuesResult, TResult> successMap)
            {
                ValuesMap = valuesMap;
                ErrorAggregator = errorAggregator;
                SuccessMap = successMap;
            }

            public Func<IEnumerable<TValue>, TValuesResult> ValuesMap { get; }
            public Func<TValuesResult, TError, TResult> ErrorAggregator { get; }
            public Func<TValuesResult, TResult> SuccessMap { get; }
        }

        private sealed class VisitVisitor<TValuesResult, TResult> : QueryResultNode.Visitor<TResult, VisitContext<TValuesResult, TResult>>
        {
            private VisitVisitor()
            {
            }

            public static VisitVisitor<TValuesResult, TResult> Instance { get; } = new VisitVisitor<TValuesResult, TResult>();

            protected internal override TResult Accept(QueryResultNode.Element node, VisitContext<TValuesResult, TResult> context)
            {
                throw new NotImplementedException();
            }

            protected internal override TResult Accept(QueryResultNode.Terminal node, VisitContext<TValuesResult, TResult> context)
            {
                throw new NotImplementedException();
            }
        }

        private abstract class QueryResultNode
        {
            private QueryResultNode()
            {
            }

            protected abstract TResult Dispatch<TResult, TContext>(QueryResultNode.Visitor<TResult, TContext> visitor, TContext context);

            public abstract class Visitor<TResult, TContext>
            {
                public TResult Visit(QueryResultNode node, TContext context)
                {
                    return node.Dispatch(this, context);
                }

                protected internal abstract TResult Accept(QueryResultNode.Element node, TContext context);
                protected internal abstract TResult Accept(QueryResultNode.Terminal node, TContext context);
            }

            public abstract class Element : QueryResultNode
            {
                public abstract TValue Value { get; }

                public abstract QueryResultNode Next();

                protected override TResult Dispatch<TResult, TContext>(QueryResultNode.Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }
            }

            public abstract class Terminal : QueryResultNode
            {
                private Terminal()
                {
                }

                protected sealed override TResult Dispatch<TResult, TContext>(QueryResultNode.Visitor<TResult, TContext> visitor, TContext context)
                {
                    return visitor.Accept(this, context);
                }

                protected abstract TResult Dispatch<TResult, TContext>(QueryResultNode.Terminal.Visitor<TResult, TContext> visitor, TContext context);

                public new abstract class Visitor<TResult, TContext>
                {
                    public TResult Visit(QueryResultNode.Terminal node, TContext context)
                    {
                        return node.Dispatch(this, context);
                    }

                    protected internal abstract TResult Accept(QueryResultNode.Terminal.Error node, TContext context);
                    protected internal abstract TResult Accept(QueryResultNode.Terminal.Empty node, TContext context);
                }

                public sealed class Error : Terminal //// TODO do you like this name?
                {
                    public Error(TError value)
                    {
                        Value = value;
                    }

                    public TError Value { get; }

                    protected override TResult Dispatch<TResult, TContext>(QueryResultNode.Terminal.Visitor<TResult, TContext> visitor, TContext context)
                    {
                        return visitor.Accept(this, context);
                    }
                }

                public sealed class Empty : Terminal //// TODO do you like this name?
                {
                    private Empty()
                    {
                    }

                    public static QueryResultNode.Terminal.Empty Instance { get; } = new QueryResultNode.Terminal.Empty();

                    protected override TResult Dispatch<TResult, TContext>(QueryResultNode.Terminal.Visitor<TResult, TContext> visitor, TContext context)
                    {
                        return visitor.Accept(this, context);
                    }
                }
            }
        }
    }
}
