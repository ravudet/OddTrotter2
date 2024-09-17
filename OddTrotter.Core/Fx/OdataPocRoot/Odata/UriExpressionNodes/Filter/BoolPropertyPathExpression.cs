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
            public EntityCollection(OdataIdentifier entityCollectionNavigationProperty, )
            {
            }
        }
    }
}
