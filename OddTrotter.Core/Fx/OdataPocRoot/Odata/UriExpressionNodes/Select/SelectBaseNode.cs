namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Select
{
    using Fx.OdataPocRoot.Odata.UriExpressionVisitors;

    public abstract class SelectBaseNode
    {
        public abstract void Accept(SelectVisitor visitor);
    }
}
