////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public sealed class GreatherThanOrEqualToExpression
    {
        public GreatherThanOrEqualToExpression(CommonExpression right)
        {
            Right = right;
        }

        public CommonExpression Right { get; }
    }
}
