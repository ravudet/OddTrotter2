namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public sealed class AliasAndValue
    {
        public AliasAndValue(OdataIdentifier parameterAlias, ParameterValue parameterValue)
        {
            ParameterAlias = parameterAlias;
            ParameterValue = parameterValue;
        }

        public OdataIdentifier ParameterAlias { get; }

        public ParameterValue ParameterValue { get; }
    }
}
