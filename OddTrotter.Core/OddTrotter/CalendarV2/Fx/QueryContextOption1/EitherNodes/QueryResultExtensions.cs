namespace Fx.QueryContextOption1.EitherNodes
{
    using System;

    using Fx.Either;

    /*public static class QueryResultExtensions
    {
        public static IQueryResult<TValue, TError> Where<TValue, TError>(this IQueryResult<TValue, TError> source, Func<TValue, bool> predicate)
        {
            source.Nodes.SelectLeft(element => predicate(element.Value) ? )
        }

        private sealed class WhereQueryResult<TValue, TError> : IQueryResult<TValue, TError>
        {
            private readonly IQueryResult<TValue, TError> source;
            private readonly Func<TValue, bool> predicate;

            public WhereQueryResult(IQueryResult<TValue, TError> source, Func<TValue, bool> predicate)
            {
                this.source = source;
                this.predicate = predicate;
            }

            public IQueryResultNode<TValue, TError> Nodes
            {
                get
                {
                    return SelectLeft(this.source.Nodes, predicate);
                    ////return SelectLeft(this.source.Nodes, this.predicate);
                }
            }

            private static QueryResultNode<TValue, TError> SelectLeft(IQueryResultNode<TValue, TError> node, Func<TValue, bool> predicate)
            {
                var result = node.SelectManyLeft(element => element.Next(), (element, next) => predicate(element.Value) ? new Element(element.Value, predicate, element.Next()) : SelectLeft(element.Next(), predicate));

                return new QueryResultNode<TValue, TError>(result);
            }

            private sealed class Element : QueryResultNode<TValue, TError>.Element
            {
                private readonly Func<TValue, bool> predicate;
                private readonly IQueryResultNode<TValue, TError> next;

                public Element(TValue value, Func<TValue, bool> predicate, IQueryResultNode<TValue, TError> next)
                {
                    Value = value;
                    this.predicate = predicate;
                    this.next = next;
                }

                public override TValue Value { get; }

                public override QueryResultNode<TValue, TError> Next()
                {

                }
            }
        }
    }*/
}
