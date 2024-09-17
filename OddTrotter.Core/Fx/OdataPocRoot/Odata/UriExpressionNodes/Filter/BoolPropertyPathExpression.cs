///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

    public abstract class BoolPropertyPathExpression
    {
        private BoolPropertyPathExpression()
        {
        }

        public sealed class EntityCollection : BoolPropertyPathExpression
        {
            public EntityCollection(
                OdataIdentifier entityCollectionNavigationProperty, 
                BoolCollectionNavigationExpression boolCollectionNavigationExpression)
            {
                EntityCollectionNavigationProperty = entityCollectionNavigationProperty;
                BoolCollectionNavigationExpression = boolCollectionNavigationExpression;
            }

            public OdataIdentifier EntityCollectionNavigationProperty { get; }

            public BoolCollectionNavigationExpression BoolCollectionNavigationExpression { get; }
        }

        public sealed class Entity : BoolPropertyPathExpression
        {
            public Entity(
                OdataIdentifier entityNavigationProperty, 
                BoolSingleNavigationExpression boolSingleNavigationExpression)
            {
                EntityNavigationProperty = entityNavigationProperty;
                BoolSingleNavigationExpression = boolSingleNavigationExpression;
            }

            public OdataIdentifier EntityNavigationProperty { get; }

            public BoolSingleNavigationExpression BoolSingleNavigationExpression { get; }
        }

        public sealed class ComplexCollection : BoolPropertyPathExpression
        {
            public ComplexCollection(
                OdataIdentifier complexCollectionProperty, 
                BoolCollectionPathExpression boolCollectionPathExpression)
            {
                ComplexCollectionProperty = complexCollectionProperty;
                BoolCollectionPathExpression = boolCollectionPathExpression;
            }

            public OdataIdentifier ComplexCollectionProperty { get; }

            public BoolCollectionPathExpression BoolCollectionPathExpression { get; }
        }

        public sealed class PrimitiveCollection : BoolPropertyPathExpression
        {
            public PrimitiveCollection(
                OdataIdentifier primitiveCollectionProperty, 
                BoolCollectionPathExpression boolCollectionPathExpression)
            {
                PrimitiveCollectionProperty = primitiveCollectionProperty;
                BoolCollectionPathExpression = boolCollectionPathExpression;
            }

            public OdataIdentifier PrimitiveCollectionProperty { get; }

            public BoolCollectionPathExpression BoolCollectionPathExpression { get; }
        }

        public abstract class PrimitveNode : BoolPropertyPathExpression
        {
            private PrimitveNode()
            {
            }

            public sealed class Primitive : PrimitveNode
            {
                public Primitive(PrimitiveProperty primitiveProperty)
                {
                    PrimitiveProperty = primitiveProperty;
                }

                public PrimitiveProperty PrimitiveProperty { get; }
            }

            public sealed class PrimitiveExpression : PrimitveNode
            {
                public PrimitiveExpression(
                    PrimitiveProperty primitiveProperty,
                    BoolPrimitivePathExpression boolPrimitivePathExpression)
                {
                    PrimitiveProperty = primitiveProperty;
                    BoolPrimitivePathExpression = boolPrimitivePathExpression;
                }

                public PrimitiveProperty PrimitiveProperty { get; }

                public BoolPrimitivePathExpression BoolPrimitivePathExpression { get; }
            }
        }

        public sealed class Stream : BoolPropertyPathExpression
        {
            public Stream(OdataIdentifier streamProperty, BoolPrimitivePathExpression boolPrimitivePathExpression)
            {
                StreamProperty = streamProperty;
                BoolPrimitivePathExpression = boolPrimitivePathExpression;
            }

            public OdataIdentifier StreamProperty { get; }

            public BoolPrimitivePathExpression BoolPrimitivePathExpression { get; }
        }
    }
}
