////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public sealed class GreaterThanExpression
    {
        public GreaterThanExpression(CommonExpression right)
        {
            Right = right;
        }

        public CommonExpression Right { get; }
    }
}
