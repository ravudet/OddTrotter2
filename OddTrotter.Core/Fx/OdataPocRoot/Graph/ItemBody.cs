namespace Fx.OdataPocRoot.Graph
{
    public sealed class ItemBody
    {
        public ItemBody(OdataProperty<string> content)
        {
            Content = content;
        }

        public OdataProperty<string> Content { get; }
    }
}
