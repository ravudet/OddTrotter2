namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

    public abstract class BoolCommonExpression
    {
        private BoolCommonExpression()
        {
        }

        public sealed class First
        {
            public First(BooleanValue booleanValue)
            {
                BooleanValue = booleanValue;
            }

            public BooleanValue BooleanValue { get; }
        }

        //// TODO do other derived types after you've created abnf for boolcommonexpr
    }
}
