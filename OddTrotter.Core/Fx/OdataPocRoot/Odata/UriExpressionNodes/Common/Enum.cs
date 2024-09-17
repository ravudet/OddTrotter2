////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public abstract class Enum
    {
        private Enum()
        {
        }

        public sealed class Qualified : Enum
        {
            public Qualified(QualifiedEnumTypeName qualifiedEnumTypeName, EnumValue enumValue)
            {
                QualifiedEnumTypeName = qualifiedEnumTypeName;
                EnumValue = enumValue;
            }

            public QualifiedEnumTypeName QualifiedEnumTypeName { get; }

            public EnumValue EnumValue { get; }
        }

        public sealed class Unqualified : Enum
        {
            public Unqualified(EnumValue enumValue)
            {
                EnumValue = enumValue;
            }

            public EnumValue EnumValue { get; }
        }
    }
}
