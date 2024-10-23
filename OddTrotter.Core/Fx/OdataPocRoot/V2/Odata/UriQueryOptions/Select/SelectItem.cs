////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Select
{
    public abstract class SelectItem
    {
        private SelectItem()
        {
            //// TODO FEATURE GAP: finish this
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Traverse(SelectItem node, TContext context)
            {
                return node.Accept(this, context);
            }
        }
    }
}
