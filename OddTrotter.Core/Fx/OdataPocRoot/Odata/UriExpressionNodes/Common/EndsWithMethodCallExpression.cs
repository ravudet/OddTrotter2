////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public sealed class EndsWithMethodCallExpression
    {
        public EndsWithMethodCallExpression(CommonExpression left, CommonExpression right)
        {
            Left = left;
            Right = right;
        }

        public CommonExpression Left { get; }

        public CommonExpression Right { get; }
    }
}
