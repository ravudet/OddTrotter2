////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public sealed class NotEqualsExpression
    {
        public NotEqualsExpression(CommonExpression right)
        {
            Right = right;
        }

        public CommonExpression Right { get; }
    }
}
