////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter;

    public sealed class OrExpression
    {
        public OrExpression(BoolCommonExpression right)
        {
            Right = right;
        }

        public BoolCommonExpression Right { get; }
    }
}
