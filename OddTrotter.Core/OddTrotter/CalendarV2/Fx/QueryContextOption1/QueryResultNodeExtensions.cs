using System;

namespace Fx.QueryContextOption1
{
    /*public static class QueryResultNodeExtensions
    {
        public static QueryResultNode<TValue, TError> Where<TValue, TError>(
            this QueryResultNode<TValue, TError> queryResultNode, 
            Func<TValue, bool> predicate)
        {
            //// TODO does enumerable start "before" the first element so that it can do laziness? your predicament below is that you have to do some amount of processing before you can know what type of node you should be returning; returning a "before first element" node would mean that you could defer that processing until the value itself is actually requested //// TODO nah, yuou can do deferrred execution with just an interface, the "Before first element" thing is to model empty sequences
            //// TODO i think i might need several more attempts at implementing query result from the bottom up before it's actually "good"
        }

        private sealed class WhereVisitor<TValue, TError> : QueryResultNode<TValue, TError>.Visitor<QueryResultNode<TValue, TError>, Func<TValue, bool>>
        {
            private WhereVisitor()
            {
            }

            public static WhereVisitor<TValue, TError> Instance { get; } = new WhereVisitor<TValue, TError>();

            protected internal override QueryResultNode<TValue, TError> Accept(QueryResultNode<TValue, TError>.Element node, Func<TValue, bool> context)
            {
            }

            protected internal override QueryResultNode<TValue, TError> Accept(QueryResultNode<TValue, TError>.Error node, Func<TValue, bool> context)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class WhereNode<TValue, TError> : QueryResultNode<TValue, TError>.Element
        {
            private readonly Element node;

            public WhereNode(QueryResultNode<TValue, TError>.Element node)
            {
                this.node = node;
            }

            public override QueryResultNode<TValue, TError> Next()
            {
                
            }
        }
    }*/
}
