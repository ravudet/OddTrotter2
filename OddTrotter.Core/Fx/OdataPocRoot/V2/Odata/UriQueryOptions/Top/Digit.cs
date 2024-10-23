////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Top
{
    public abstract class Digit
    {
        private Digit()
        {
            //// TODO FEATURE GAP: finish this
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Traverse(Digit node, TContext context)
            {
                return node.Accept(this, context);
            }
        }
    }
}
