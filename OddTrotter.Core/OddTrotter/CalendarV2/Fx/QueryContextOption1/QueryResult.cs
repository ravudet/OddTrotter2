namespace Fx.QueryContextOption1
{
    /*using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public abstract class QueryResult<TValue, TError>
    {
        private QueryResult()
        {
            //// TODO is this an `either`? //// TODO this may be an either, but you don't know left or right until you've started enumerating; maybe you should have a `queryresult` which has a `ienumerable<queryresultnode>`?
            //// TODO because there's the `ieither` interface now, you might actually be able to have `queryresult : ieither` //// TODO this really just keeps coming back to: you have to enumerate the values to get the error; i'm choosing to now allow the caller to ignore this because i don't know what to do with the realized values that we had to enumerate before the error occurred; it's not guaranteed that these will be repeatable; //// TODO you *could* have the two options for either be ienumerable<value> and queryresultnode<value, error> and in all of your methods don't allow a different func to be applied to the values; but then you won't be really implementing the `apply` method in `ieither`; but that *might* be a real use case that you're not really thinking of (i.e. i need to do something different with each element if it turns out to be eventually result in an error); write out an example of this and see which option looks better //// TODO is there a way that you can (for the error case) accept a map for the enumerable of values and a map for the error, that way the *caller* isn't the one having to do things in the right order? //// TODO yes, you can have an apply method on the `partial` node and never have it expose those properties directly
            //// TODO can `queryresultnode` be a ref struct? and then you can have it allow ref struct too; you don't need to have dynamically allocated memory once you've received the network response; or rather, you can allocate the memory on the stack as you're streaming the network response
            //// TODO can you have a select into a ref struct? how about from a ref struct?
            //// TODO use `in` parameters for the `context` in all visitors?
            //// TODO do you need an interface?
        }

        protected abstract TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, in TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Visit(QueryResult<TValue, TError> node, in TContext context)
            {
                if (node == null)
                {
                    throw new ArgumentNullException(nameof(node));
                }

                return node.Dispatch(this, context);
            }

            public abstract TResult Accept(QueryResult<TValue, TError>.Full node, in TContext context);
            public abstract TResult Accept(QueryResult<TValue, TError>.Partial node, in TContext context);
        }

        protected abstract Task<TResult> DispatchAsync<TResult, TContext>(
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

                return await node.DispatchAsync(this, context).ConfigureAwait(false);
            }

            public abstract Task<TResult> AcceptAsync(QueryResult<TValue, TError>.Full node, in TContext context);
            public abstract Task<TResult> AcceptAsync(QueryResult<TValue, TError>.Partial node, in TContext context);
        }

        public sealed class Full : QueryResult<TValue, TError>
        {
            public Full(IEnumerable<TValue> values)
            {
                this.Values = values;
            }

            public IEnumerable<TValue> Values { get; }

            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, in TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return visitor.Accept(this, context);
            }

            protected override async Task<TResult> DispatchAsync<TResult, TContext>(AsyncVisitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return await visitor.AcceptAsync(this, context).ConfigureAwait(false);
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

            protected override TResult Dispatch<TResult, TContext>(Visitor<TResult, TContext> visitor, in TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return visitor.Accept(this, context);
            }

            protected override async Task<TResult> DispatchAsync<TResult, TContext>(AsyncVisitor<TResult, TContext> visitor, TContext context)
            {
                if (visitor == null)
                {
                    throw new ArgumentNullException(nameof(visitor));
                }

                return await visitor.AcceptAsync(this, context).ConfigureAwait(false);
            }
        }
    }*/
}
