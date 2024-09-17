////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public sealed class QualifiedEnumTypeName
    {
        public QualifiedEnumTypeName(Namespace @namespace, OdataIdentifier enumerationTypeName)
        {
            Namespace = @namespace;
            EnumerationTypeName = enumerationTypeName;
        }

        public Namespace Namespace { get; }

        public OdataIdentifier EnumerationTypeName { get; }
    }
}
