////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter;

    public sealed class AndExpression
    {
        public AndExpression(BoolCommonExpression right)
        {
            Right = right;
        }

        public BoolCommonExpression Right { get; }
    }
}
