using Fx.OdataPocRoot.Odata;

namespace Fx.OdataPocRoot.Graph
{
    public sealed class ItemBody
    {
        public ItemBody(OdataProperty<string> content)
        {
            Content = content;
        }

        [PropertyName("content")]
        public OdataProperty<string> Content { get; }
    }
}
