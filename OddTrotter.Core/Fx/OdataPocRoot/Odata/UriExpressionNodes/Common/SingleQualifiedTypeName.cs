////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public abstract class SingleQualifiedTypeName
    {
        private SingleQualifiedTypeName()
        {
        }

        public sealed class QualifiedEntityType : SingleQualifiedTypeName
        {
            public QualifiedEntityType(QualifiedEntityTypeName qualifiedEntityTypeName)
            {
                QualifiedEntityTypeName = qualifiedEntityTypeName;
            }

            public QualifiedEntityTypeName QualifiedEntityTypeName { get; }
        }

        public sealed class QualifiedComplexType : SingleQualifiedTypeName
        {
            public QualifiedComplexType(QualifiedComplexTypeName qualifiedComplexTypeName)
            {
                QualifiedComplexTypeName = qualifiedComplexTypeName;
            }

            public QualifiedComplexTypeName QualifiedComplexTypeName { get; }
        }

        public sealed class QualifiedTypeDefinition : SingleQualifiedTypeName
        {
            public QualifiedTypeDefinition(QualifiedTypeDefinitionName qualifiedTypeDefinitionName)
            {
                QualifiedTypeDefinitionName = qualifiedTypeDefinitionName;
            }

            public QualifiedTypeDefinitionName QualifiedTypeDefinitionName { get; }
        }

        public sealed class QualifiedEnumType : SingleQualifiedTypeName
        {
            public QualifiedEnumType(QualifiedEnumTypeName qualifiedEnumTypeName)
            {
                QualifiedEnumTypeName = qualifiedEnumTypeName;
            }

            public QualifiedEnumTypeName QualifiedEnumTypeName { get; }
        }

        public sealed class PrimitiveType : SingleQualifiedTypeName
        {
            public PrimitiveType(PrimitiveTypeName primitiveTypeName)
            {
                PrimitiveTypeName = primitiveTypeName;
            }

            public PrimitiveTypeName PrimitiveTypeName { get; }
        }
    }
}
