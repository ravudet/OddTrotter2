///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    using System.Collections.Generic;

    public sealed class EnumValue
    {
        public EnumValue(IEnumerable<SingleEnumValue> singleEnumValues)
        {
            //// TODO actually has to have at least one; check all collections in these nodes for this case and model them appropriately
            SingleEnumValues = singleEnumValues;
        }

        public IEnumerable<SingleEnumValue> SingleEnumValues { get; }
    }
}
