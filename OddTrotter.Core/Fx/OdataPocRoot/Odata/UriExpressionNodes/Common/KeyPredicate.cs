///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public abstract class KeyPredicate
    {
        private KeyPredicate()
        {
        }

        public abstract class SimpleKey : KeyPredicate
        {
            private SimpleKey()
            {
            }

            public sealed class ParameterAliasNode : SimpleKey
            {
                public ParameterAliasNode(OdataIdentifier parameterAlias)
                {
                    ParameterAlias = parameterAlias;
                }

                public OdataIdentifier ParameterAlias { get; }
            }

            public sealed class KeyPropertyValueNode : SimpleKey
            {
                public KeyPropertyValueNode(PrimitiveLiteral primitiveLiteral)
                {
                    PrimitiveLiteral = primitiveLiteral;
                }

                public PrimitiveLiteral PrimitiveLiteral { get; }
            }
        }
    }
}
