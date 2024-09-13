namespace Fx.OdataPocRoot.Odata.UriExpressionVisitors
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Select;

    public abstract class SelectVisitor
    {
        public void Visit(SelectBaseNode node)
        {
            node.Accept(this);
        }

        public abstract void Visit(Select node);

        public abstract void Visit(SelectItem node);

        public abstract void Visit(SelectOption node);

        public abstract void Visit(SelectOptionPc node);

        public abstract void Visit(SelectPath node);

        public abstract void Visit(SelectProperty node);
    }
}
