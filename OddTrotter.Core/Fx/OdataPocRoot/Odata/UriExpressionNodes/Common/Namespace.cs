namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    using System.Collections.Generic;

    public sealed class Namespace
    {
        public Namespace(IEnumerable<OdataIdentifier> namespaceParts)
        {
            NamespaceParts = namespaceParts;
        }

        public IEnumerable<OdataIdentifier> NamespaceParts { get; }
    }
}
