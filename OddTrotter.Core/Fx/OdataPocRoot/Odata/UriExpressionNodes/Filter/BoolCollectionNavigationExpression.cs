///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

    public abstract class BoolCollectionNavigationExpression
    {
        private BoolCollectionNavigationExpression()
        {
        }

        public sealed class First : BoolCollectionNavigationExpression
        {
            public First(
                QualifiedEntityTypeName qualifiedEntityTypeName, 
                KeyPredicate keyPredicate, 
                BoolSingleNavigationExpression boolSingleNavigationExpression)
            {
                QualifiedEntityTypeName = qualifiedEntityTypeName;
                KeyPredicate = keyPredicate;
                BoolSingleNavigationExpression = boolSingleNavigationExpression;
            }

            public QualifiedEntityTypeName QualifiedEntityTypeName { get; }

            public KeyPredicate KeyPredicate { get; }

            public BoolSingleNavigationExpression BoolSingleNavigationExpression { get; }
        }

        public sealed class Second : BoolCollectionNavigationExpression
        {
            public Second(
                QualifiedEntityTypeName qualifiedEntityTypeName, 
                BoolCollectionPathExpression boolCollectionPathExpression)
            {
                QualifiedEntityTypeName = qualifiedEntityTypeName;
                BoolCollectionPathExpression = boolCollectionPathExpression;
            }

            public QualifiedEntityTypeName QualifiedEntityTypeName { get; }

            public BoolCollectionPathExpression BoolCollectionPathExpression { get; }
        }

        public sealed class Third : BoolCollectionNavigationExpression
        {
            public Third(KeyPredicate keyPredicate, BoolSingleNavigationExpression boolSingleNavigationExpression)
            {
                KeyPredicate = keyPredicate;
                BoolSingleNavigationExpression = boolSingleNavigationExpression;
            }

            public KeyPredicate KeyPredicate { get; }

            public BoolSingleNavigationExpression BoolSingleNavigationExpression { get; }
        }

        public sealed class Fourth : BoolCollectionNavigationExpression
        {
            public Fourth(BoolCollectionPathExpression boolCollectionPathExpression)
            {
                BoolCollectionPathExpression = boolCollectionPathExpression;
            }

            public BoolCollectionPathExpression BoolCollectionPathExpression { get; }
        }
    }
}
