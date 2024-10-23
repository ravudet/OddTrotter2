namespace Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter
{
    public abstract class RootExpr
    {
        private RootExpr()
        {
            //// TODO FEATURE GAP: finish this
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Traverse(RootExpr node, TContext context)
            {
                return node.Accept(this, context);
            }
        }
    }
}
