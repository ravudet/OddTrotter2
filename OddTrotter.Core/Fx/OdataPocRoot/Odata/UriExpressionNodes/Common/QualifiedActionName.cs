namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public sealed class QualifiedActionName
    {
        public QualifiedActionName(Namespace @namespace, OdataIdentifier action)
        {
            Namespace = @namespace;
            Action = action;
        }

        public Namespace Namespace { get; }

        public OdataIdentifier Action { get; }
    }
}
