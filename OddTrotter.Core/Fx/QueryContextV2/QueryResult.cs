namespace Fx.QueryContextV2
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

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
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                return node.Accept(this, context);
            }

            public abstract TResult Dispatch(Full node, TContext context);
            public abstract TResult Dispatch(Partial node, TContext context);
        }

        protected abstract Task<TResult> AcceptAsync<TResult, TContext>(
            AsyncVisitor<TResult, TContext> visitor, 
            TContext context);

        public abstract class AsyncVisitor<TResult, TContext>
        {
            public async Task<TResult> VisitAsync(QueryResult<TValue, TError> node, TContext context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                return await node.AcceptAsync(this, context).ConfigureAwait(false);
            }

            public abstract Task<TResult> DispatchAsync(Full node, TContext context);
            public abstract Task<TResult> DispatchAsync(Partial node, TContext context);
        }

        public sealed class Full : QueryResult<TValue, TError>
        {
            public Full(IEnumerable<TValue> values)
            {
                this.Values = values;
            }

            public IEnumerable<TValue> Values { get; }

            protected override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return visitor.Dispatch(this, context);
            }

            protected override async Task<TResult> AcceptAsync<TResult, TContext>(AsyncVisitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return await visitor.DispatchAsync(this, context).ConfigureAwait(false);
            }
        }

        public sealed class Partial : QueryResult<TValue, TError>
        {
            public Partial(IEnumerable<TValue> values, TError error)
            {
                this.Values = values;
                this.Error = error;
            }

            public IEnumerable<TValue> Values { get; } //// TODO do you want this on the base type?
            public TError Error { get; }

            protected override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return visitor.Dispatch(this, context);
            }

            protected override async Task<TResult> AcceptAsync<TResult, TContext>(AsyncVisitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return await visitor.DispatchAsync(this, context).ConfigureAwait(false);
            }
        }
    }
}
