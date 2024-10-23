////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter
{
    public sealed class Filter
    {
        public Filter(BoolCommonExpression boolCommonExpression)
        {
            this.BoolCommonExpression = boolCommonExpression;
        }

        public BoolCommonExpression BoolCommonExpression { get; }
    }
}
