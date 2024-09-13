using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

namespace Fx.OdataPocRoot.Odata.UriExpressionVisitors
{
    public abstract class CommonVisitor
    {
        public void Visit(CommonBaseNode node)
        {
            node.Accept(this);
        }

        public abstract void Visit(AliasAndValue node);

        public abstract void Visit(Namespace node);

        public abstract void Visit(NavigationProperty node);

        public abstract void Visit(OdataIdentifier node);

        public abstract void Visit(ParameterValue node);

        public abstract void Visit(PrimitiveProperty node);

        public abstract void Visit(QualifiedActionName node);

        public abstract void Visit(QualifiedComplexTypeName node);

        public abstract void Visit(QualifiedEntityTypeName node);

        public abstract void Visit(QualifiedFunctionName node);
    }
}
