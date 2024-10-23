////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Top
{
    using global::System.Collections.Generic;

    public sealed class Top
    {
        public Top(Digit digit, IEnumerable<Digit> digits)
        {
            this.Digit = digit;
            this.Digits = digits;
        }

        public Digit Digit { get; }

        public IEnumerable<Digit> Digits { get; }
    }
}
