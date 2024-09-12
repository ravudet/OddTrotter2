namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public abstract class PrimitiveProperty
    {
        private PrimitiveProperty()
        {
        }

        public sealed class PrimitiveKeyProperty : PrimitiveProperty
        {
            public PrimitiveKeyProperty(OdataIdentifier identifier)
            {
                Identifier = identifier;
            }

            public OdataIdentifier Identifier { get; }
        }

        public sealed class PrimitiveNonKeyProperty : PrimitiveProperty
        {
            public PrimitiveNonKeyProperty(OdataIdentifier identifier)
            {
                Identifier = identifier;
            }

            public OdataIdentifier Identifier { get; }
        }
    }
}
