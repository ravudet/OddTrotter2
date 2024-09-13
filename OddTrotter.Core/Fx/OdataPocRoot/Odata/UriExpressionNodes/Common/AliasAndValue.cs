using Fx.OdataPocRoot.Odata.UriExpressionVisitors;

namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public sealed class AliasAndValue : CommonBaseNode
    {
        public AliasAndValue(OdataIdentifier parameterAlias, ParameterValue parameterValue)
        {
            ParameterAlias = parameterAlias;
            ParameterValue = parameterValue;
        }

        public OdataIdentifier ParameterAlias { get; }

        public ParameterValue ParameterValue { get; }

        public override void Accept(CommonVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
