////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter
{
    public sealed class ParenExpr
    {
        public ParenExpr(CommonExpression commonExpression)
        {
            this.CommonExpression = commonExpression;
        }

        public CommonExpression CommonExpression { get; }
    }
}
