////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter;

    public sealed class NotExpression
    {
        public NotExpression(BoolCommonExpression boolCommonExpression)
        {
            BoolCommonExpression = boolCommonExpression;
        }

        public BoolCommonExpression BoolCommonExpression { get; }
    }
}
