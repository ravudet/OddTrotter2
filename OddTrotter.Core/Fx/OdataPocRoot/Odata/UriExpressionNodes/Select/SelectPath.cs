namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Select
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

    public abstract class SelectPath
    {
        private SelectPath()
        {
        }

        public sealed class First : SelectPath
        {
            public First(OdataIdentifier complexProperty)
            {
                ComplexProperty = complexProperty;
            }

            public OdataIdentifier ComplexProperty { get; }
        }

        public sealed class Second : SelectPath
        {
            public Second(OdataIdentifier complexProperty, QualifiedComplexTypeName qualifiedComplexTypeName)
            {
                ComplexProperty = complexProperty;
                QualifiedComplexTypeName = qualifiedComplexTypeName;
            }

            public OdataIdentifier ComplexProperty { get; }

            public QualifiedComplexTypeName QualifiedComplexTypeName { get; }
        }

        public sealed class Third : SelectPath
        {
            public Third(OdataIdentifier complexCollectionProperty)
            {
                ComplexCollectionProperty = complexCollectionProperty;
            }

            public OdataIdentifier ComplexCollectionProperty { get; }
        }

        public sealed class Fourth : SelectPath
        {
            public Fourth(OdataIdentifier complexCollectionProperty, QualifiedComplexTypeName qualifiedComplexTypeName)
            {
                ComplexCollectionProperty = complexCollectionProperty;
                QualifiedComplexTypeName = qualifiedComplexTypeName;
            }

            public OdataIdentifier ComplexCollectionProperty { get; }

            public QualifiedComplexTypeName QualifiedComplexTypeName { get; }
        }
    }
}
