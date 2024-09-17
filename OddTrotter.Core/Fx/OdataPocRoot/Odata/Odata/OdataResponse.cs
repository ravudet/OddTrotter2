////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata
{
    using System.IO;

    public abstract class OdataResponse
    {
        private OdataResponse()
        {
        }

        public sealed class Instance : OdataResponse
        {
            public Instance(Stream contents)
            {
                Contents = contents;
            }

            public Stream Contents { get; } //// TODO make this the AST for an odata instance response payload
        }

        public sealed class Collection : OdataResponse
        {
            public Collection(Stream contents)
            {
                Contents = contents;
            }

            public Stream Contents { get; } //// TODO make this the AST for an odata collection response payload
        }
    }
}
