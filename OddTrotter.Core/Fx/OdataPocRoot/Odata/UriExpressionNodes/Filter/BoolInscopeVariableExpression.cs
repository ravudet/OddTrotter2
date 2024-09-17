////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

    public abstract class BoolInscopeVariableExpression
    {
        private BoolInscopeVariableExpression()
        {
        }

        public sealed class ImplicitVariable : BoolInscopeVariableExpression
        {
            public ImplicitVariable(ImplicitVariableExpression implicitVariableExpression)
            {
                ImplicitVariableExpression = implicitVariableExpression;
            }

            public ImplicitVariableExpression ImplicitVariableExpression { get; }
        }

        public sealed class ParameterAliasNode : BoolInscopeVariableExpression
        {
            public ParameterAliasNode(OdataIdentifier parameterAlias)
            {
                ParameterAlias = parameterAlias;
            }

            public OdataIdentifier ParameterAlias { get; }
        }
    }
}
