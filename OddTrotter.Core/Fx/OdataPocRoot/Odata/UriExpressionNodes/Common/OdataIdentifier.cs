namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public sealed class OdataIdentifier
    {
        public OdataIdentifier(string identifier)
        {
            //// TODO use a regex match
            this.Identifier = identifier;
        }

        public string Identifier { get; }
    }
}
