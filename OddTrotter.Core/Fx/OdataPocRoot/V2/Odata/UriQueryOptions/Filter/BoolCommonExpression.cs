////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter
{
    public sealed class BoolCommonExpression
    {
        public BoolCommonExpression(CommonExpression commonExpression)
        {
            this.CommonExpression = commonExpression;
        }

        public CommonExpression CommonExpression { get; }
    }
}
