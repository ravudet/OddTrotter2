///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter
{
    public sealed class BoolSingleNavigationExpression
    {
        public BoolSingleNavigationExpression(BoolMemberExpression boolMemberExpression)
        {
            BoolMemberExpression = boolMemberExpression;
        }

        public BoolMemberExpression BoolMemberExpression { get; }
    }
}
