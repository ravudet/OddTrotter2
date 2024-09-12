namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public abstract class NavigationProperty
    {
        private NavigationProperty()
        {
        }

        public sealed class EntityNavigationProperty : NavigationProperty
        {
            public EntityNavigationProperty(OdataIdentifier identifier)
            {
                Identifier = identifier;
            }

            public OdataIdentifier Identifier { get; }
        }

        public sealed class EntityCollectionNavigationProperty : NavigationProperty
        {
            public EntityCollectionNavigationProperty(OdataIdentifier identifier)
            {
                Identifier = identifier;
            }

            public OdataIdentifier Identifier { get; }
        }
    }
}
