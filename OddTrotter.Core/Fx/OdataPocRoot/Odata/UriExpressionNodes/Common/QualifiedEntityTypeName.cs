namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public sealed class QualifiedEntityTypeName
    {
        public QualifiedEntityTypeName(Namespace @namespace, OdataIdentifier entityTypeName)
        {
            Namespace = @namespace;

            EntityTypeName = entityTypeName;
        }

        public Namespace Namespace { get; }

        public OdataIdentifier EntityTypeName { get; }
    }
}
