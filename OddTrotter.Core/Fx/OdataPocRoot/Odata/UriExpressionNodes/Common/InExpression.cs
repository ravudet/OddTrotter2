////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public sealed class InExpression
    {
        public InExpression(CommonExpression right)
        {
            Right = right;
        }

        public CommonExpression Right { get; }
    }
}
