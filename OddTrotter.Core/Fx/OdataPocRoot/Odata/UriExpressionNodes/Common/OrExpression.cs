////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter;

    public sealed class OrExpression
    {
        public OrExpression(BoolCommonExpression boolCommonExpression)
        {
            BoolCommonExpression = boolCommonExpression;
        }

        public BoolCommonExpression BoolCommonExpression { get; }
    }
}
