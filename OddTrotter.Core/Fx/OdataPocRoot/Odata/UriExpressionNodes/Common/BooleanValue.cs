namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public abstract class BooleanValue
    {
        private BooleanValue()
        {
        }

        public sealed class True : BooleanValue
        {
            public True()
            {
                //// TODO singleton?
            }
        }

        public sealed class False : BooleanValue
        {
            public False()
            {
                //// TODO singleton?
            }
        }
    }
}
