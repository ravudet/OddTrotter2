namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public sealed class IntersectsMethodCallExpression
    {
        public IntersectsMethodCallExpression(CommonExpression left, CommonExpression right)
        {
            Left = left;
            Right = right;
        }

        public CommonExpression Left { get; }

        public CommonExpression Right { get; }
    }
}
