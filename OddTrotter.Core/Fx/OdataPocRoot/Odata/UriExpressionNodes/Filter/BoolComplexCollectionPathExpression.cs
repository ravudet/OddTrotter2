///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter
{
    public abstract class BoolComplexCollectionPathExpression
    {
        private BoolComplexCollectionPathExpression()
        {
        }

        public sealed class Qualified : BoolComplexCollectionPathExpression
        {
            public Qualified(
                QualifiedComplexTypeName qualifiedComplexTypeName, 
                BoolCollectionPathExpression boolCollectionPathExpression)
            {
                QualifiedComplexTypeName = qualifiedComplexTypeName;
                BoolCollectionPathExpression = boolCollectionPathExpression;
            }

            public QualifiedComplexTypeName QualifiedComplexTypeName { get; }

            public BoolCollectionPathExpression BoolCollectionPathExpression { get; }
        }

        public sealed class Unqualified : BoolComplexCollectionPathExpression
        {
            public Unqualified(BoolCollectionPathExpression boolCollectionPathExpression)
            {
                BoolCollectionPathExpression = boolCollectionPathExpression;
            }

            public BoolCollectionPathExpression BoolCollectionPathExpression { get; }
        }
    }
}
