////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public sealed class LessThanExpression
    {
        public LessThanExpression(CommonExpression right)
        {
            Right = right;
        }

        public CommonExpression Right { get; }
    }
}
