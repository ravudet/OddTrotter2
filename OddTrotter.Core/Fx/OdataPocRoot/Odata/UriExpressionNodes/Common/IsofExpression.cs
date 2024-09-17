////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public sealed class IsofExpression
    {
        public IsofExpression(CommonExpression commonExpression, QualifiedTypeName qualifiedTypeName)
        {
            CommonExpression = commonExpression;
            QualifiedTypeName = qualifiedTypeName;
        }

        public CommonExpression CommonExpression { get; }

        public QualifiedTypeName QualifiedTypeName { get; }
    }
}
