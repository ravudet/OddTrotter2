namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    using Fx.OdataPocRoot.Odata.UriExpressionVisitors;

    public abstract class CommonBaseNode //// TODO better name
    {
        public abstract void Accept(CommonVisitor visitor);
    }
}
