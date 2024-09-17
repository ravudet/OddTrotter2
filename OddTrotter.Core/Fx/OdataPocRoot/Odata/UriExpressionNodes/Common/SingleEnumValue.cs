///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public abstract class SingleEnumValue
    {
        private SingleEnumValue()
        {
        }

        public sealed class EnumerationMemberNode : SingleEnumValue
        {
            public EnumerationMemberNode(OdataIdentifier enumerationMember)
            {
                EnumerationMember = enumerationMember;
            }

            public OdataIdentifier EnumerationMember { get; }
        }

        public sealed class EnumMemberValueNode : SingleEnumValue
        {
            public EnumMemberValueNode(long enumMemberValue)
            {
                EnumMemberValue = enumMemberValue;
            }

            public long EnumMemberValue { get; }
        }
    }
}
