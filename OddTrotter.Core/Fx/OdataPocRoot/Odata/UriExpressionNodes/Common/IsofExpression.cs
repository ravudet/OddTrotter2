////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public sealed class IsofExpression
    {
        //// TODO you seem to be missing the case where there is no commonexpression

        public IsofExpression(CommonExpression commonExpression, QualifiedTypeName qualifiedTypeName)
        {
            CommonExpression = commonExpression;
            QualifiedTypeName = qualifiedTypeName;
        }

        public CommonExpression CommonExpression { get; }

        public QualifiedTypeName QualifiedTypeName { get; }
    }
}
