namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter
{
    public sealed class Filter
    {
        //// TODO do this
        public Filter(BoolCommonExpression boolCommonExpression)
        {
            this.BoolCommonExpression = boolCommonExpression;
        }

        public BoolCommonExpression BoolCommonExpression { get; }
    }
}
