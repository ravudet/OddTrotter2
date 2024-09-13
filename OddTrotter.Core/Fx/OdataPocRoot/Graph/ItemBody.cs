namespace Fx.OdataPocRoot.Graph
{
    public sealed class ItemBody
    {
        public ItemBody(string content)
        {
            Content = new OdataProperty<string>(content);
        }

        public OdataProperty<string> Content { get; }
    }
}
