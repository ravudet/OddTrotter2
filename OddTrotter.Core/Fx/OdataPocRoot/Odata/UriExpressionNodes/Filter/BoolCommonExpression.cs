namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

    //// TODO finish the rest of boolCommonExpr.abnf
    public abstract class BoolCommonExpression
    {
        private BoolCommonExpression()
        {
        }

        public sealed class First : BoolCommonExpression
        {
            public First(BooleanValue booleanValue)
            {
                BooleanValue = booleanValue;
            }

            public BooleanValue BooleanValue { get; }
        }

        public sealed class Second : BoolCommonExpression
        {
            public Second(BoolCommonExpression boolCommonExpression, AndExpression andExpression)
            {
                BoolCommonExpression = boolCommonExpression;
                AndExpression = andExpression;
            }

            public BoolCommonExpression BoolCommonExpression { get; }

            public AndExpression AndExpression { get; }
        }

        //// TODO do other derived types after you've created abnf for boolcommonexpr
    }

    public sealed class AndExpression
    {
        public AndExpression(BoolCommonExpression right)
        {
            Right = right;
        }

        public BoolCommonExpression Right { get; }
    }
}