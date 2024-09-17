///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

    public abstract class BoolMemberExpression
    {
        private BoolMemberExpression()
        {
        }

        public sealed class First : BoolMemberExpression
        {
            public First(QualifiedEntityTypeName qualifiedEntityTypeName, BoolProperty)
        }
    }
}
