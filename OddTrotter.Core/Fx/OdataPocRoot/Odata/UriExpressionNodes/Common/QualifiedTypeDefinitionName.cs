////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public sealed class QualifiedTypeDefinitionName
    {
        public QualifiedTypeDefinitionName(Namespace @namespace, OdataIdentifier typeDefinitionName)
        {
            Namespace = @namespace;
            TypeDefinitionName = typeDefinitionName;
        }

        public Namespace Namespace { get; }

        public OdataIdentifier TypeDefinitionName { get; }
    }
}
