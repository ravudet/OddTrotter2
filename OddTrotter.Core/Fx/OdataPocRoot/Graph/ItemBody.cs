using Fx.OdataPocRoot.Odata;

namespace Fx.OdataPocRoot.Graph
{
    public sealed class ItemBody
    {
        public ItemBody(OdataInstanceProperty<string> content)
        {
            Content = content;
        }

        [PropertyName("content")]
        public OdataInstanceProperty<string> Content { get; }
    }
}
