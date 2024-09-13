namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Select
{
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

    public abstract class SelectItem
    {
        private SelectItem()
        {
        }

        public sealed class Star : SelectItem
        {
            private Star()
            {
            }

            public static Star Instance { get; } = new Star();
        }

        public sealed class AllOperationsInSchema : SelectItem
        {
            public AllOperationsInSchema(Namespace schemaNamespace)
            {
                SchemaNamespace = schemaNamespace;
            }

            public Namespace SchemaNamespace { get; }
        }

        public abstract class PropertyPath : SelectItem
        {
            private PropertyPath()
            {
            }

            /// <summary>
            /// TODO come up with a better name for each of these derived types
            /// </summary>
            public sealed class First : PropertyPath
            {
                public First(QualifiedEntityTypeName qualifiedEntityTypeName, SelectProperty selectProperty)
                {
                    QualifiedEntityTypeName = qualifiedEntityTypeName;
                    SelectProperty = selectProperty;
                }

                public QualifiedEntityTypeName QualifiedEntityTypeName { get; }

                public SelectProperty SelectProperty { get; }
            }

            public sealed class Second : PropertyPath
            {
                public Second(SelectProperty selectProperty)
                {
                    SelectProperty = selectProperty;
                }

                public SelectProperty SelectProperty { get; }
            }

            public sealed class Third : PropertyPath
            {
                public Third(QualifiedEntityTypeName qualifiedEntityTypeName, QualifiedActionName qualifiedActionName)
                {
                    QualifiedEntityTypeName = qualifiedEntityTypeName;
                    QualifiedActionName = qualifiedActionName;
                }

                public QualifiedEntityTypeName QualifiedEntityTypeName { get; }

                public QualifiedActionName QualifiedActionName { get; }
            }

            public sealed class Fourth : PropertyPath
            {
                public Fourth(QualifiedActionName qualifiedActionName)
                {
                    QualifiedActionName = qualifiedActionName;
                }

                public QualifiedActionName QualifiedActionName { get; }
            }

            public sealed class Fifth : PropertyPath
            {
                public Fifth(QualifiedEntityTypeName qualifiedEntityTypeName, QualifiedFunctionName qualifiedFunctionName)
                {
                    QualifiedEntityTypeName = qualifiedEntityTypeName;
                    QualifiedFunctionName = qualifiedFunctionName;
                }

                public QualifiedEntityTypeName QualifiedEntityTypeName { get; }

                public QualifiedFunctionName QualifiedFunctionName { get; }
            }

            public sealed class Sixth : PropertyPath
            {
                public Sixth(QualifiedFunctionName qualifiedFunctionName)
                {
                    QualifiedFunctionName = qualifiedFunctionName;
                }

                public QualifiedFunctionName QualifiedFunctionName { get; }
            }

            public sealed class Seventh : PropertyPath
            {
                public Seventh(QualifiedComplexTypeName qualifiedComplexTypeName, SelectProperty selectProperty)
                {
                    QualifiedComplexTypeName = qualifiedComplexTypeName;
                    SelectProperty = selectProperty;
                }

                public QualifiedComplexTypeName QualifiedComplexTypeName { get; }

                public SelectProperty SelectProperty { get; }
            }

            public sealed class Eighth : PropertyPath
            {
                public Eighth(QualifiedComplexTypeName qualifiedComplexTypeName, QualifiedActionName qualifiedActionName)
                {
                    QualifiedComplexTypeName = qualifiedComplexTypeName;
                    QualifiedActionName = qualifiedActionName;
                }

                public QualifiedComplexTypeName QualifiedComplexTypeName { get; }

                public QualifiedActionName QualifiedActionName { get; }
            }

            public sealed class Ninth : PropertyPath
            {
                public Ninth(QualifiedComplexTypeName qualifiedComplexTypeName, QualifiedFunctionName qualifiedFunctionName)
                {
                    QualifiedComplexTypeName = qualifiedComplexTypeName;

                    QualifiedFunctionName = qualifiedFunctionName;
                }

                public QualifiedComplexTypeName QualifiedComplexTypeName { get; }

                public QualifiedFunctionName QualifiedFunctionName { get; }
            }
        }
    }
}
