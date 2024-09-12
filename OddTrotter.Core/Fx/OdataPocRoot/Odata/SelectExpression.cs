namespace Fx.OdataPocRoot.Odata
{
    using System.Collections.Generic;
    using System.Threading;

    public sealed class SelectExpression
    {
        public IReadOnlyList<SelectExpressionNode> Nodes { get; }
    }

    public abstract class SelectExpressionNode
    {
        private SelectExpressionNode()
        {
        }

        public sealed class Star : SelectExpressionNode
        {
            private Star()
            {
            }

            public static Star Instance { get; } = new Star();
        }

        public sealed class AllOperationsInSchema : SelectExpressionNode
        {
            public AllOperationsInSchema(OdataUriNode.NamespaceNode schemaNamespace)
            {
                SchemaNamespace = schemaNamespace;
            }

            public OdataUriNode.NamespaceNode SchemaNamespace { get; }
        }

        public abstract class PropertyPath : SelectExpressionNode
        {
            private PropertyPath()
            {
            }

            /// <summary>
            /// TODO come up with a better name for each of these derived types
            /// </summary>
            public sealed class First : PropertyPath
            {

                public First(OdataUriNode.QualifiedEntityTypeName qualifiedEntityTypeName, SelectPropertyNode selectProperty)
                {
                    QualifiedEntityTypeName = qualifiedEntityTypeName;
                    SelectProperty = selectProperty;
                }

                public OdataUriNode.QualifiedEntityTypeName QualifiedEntityTypeName { get; }

                public SelectPropertyNode SelectProperty { get; }
            }

            public sealed class Second : PropertyPath
            {
                public Second(SelectPropertyNode selectProperty)
                {
                    SelectProperty = selectProperty;
                }

                public SelectPropertyNode SelectProperty { get; }
            }

            public sealed class Third : PropertyPath
            {
                public Third(OdataUriNode.QualifiedEntityTypeName qualifiedEntityTypeName, OdataUriNode.QualifiedActionName qualifiedActionName)
                {
                    QualifiedEntityTypeName = qualifiedEntityTypeName;
                    QualifiedActionName = qualifiedActionName;
                }

                public OdataUriNode.QualifiedEntityTypeName QualifiedEntityTypeName { get; }

                public OdataUriNode.QualifiedActionName QualifiedActionName { get; }
            }

            public sealed class Fourth : PropertyPath
            {
                public Fourth(OdataUriNode.QualifiedActionName qualifiedActionName)
                {
                    QualifiedActionName = qualifiedActionName;
                }

                public OdataUriNode.QualifiedActionName QualifiedActionName { get; }
            }

            public sealed class Fifth : PropertyPath
            {
                public Fifth(OdataUriNode.QualifiedEntityTypeName qualifiedEntityTypeName, OdataUriNode.QualifiedFunctionName qualifiedFunctionName)
                {
                    QualifiedEntityTypeName = qualifiedEntityTypeName;
                    QualifiedFunctionName = qualifiedFunctionName;
                }

                public OdataUriNode.QualifiedEntityTypeName QualifiedEntityTypeName { get; }
                
                public OdataUriNode.QualifiedFunctionName QualifiedFunctionName { get; }
            }

            public sealed class Sixth : PropertyPath
            {
                public Sixth(OdataUriNode.QualifiedFunctionName qualifiedFunctionName)
                {
                    QualifiedFunctionName = qualifiedFunctionName;
                }

                public OdataUriNode.QualifiedFunctionName QualifiedFunctionName { get; }
            }

            public sealed class Seventh : PropertyPath
            {
                public Seventh(OdataUriNode.QualifiedComplexTypeName qualifiedComplexTypeName, SelectPropertyNode selectProperty)
                {
                    QualifiedComplexTypeName = qualifiedComplexTypeName;
                    SelectProperty = selectProperty;
                }

                public OdataUriNode.QualifiedComplexTypeName QualifiedComplexTypeName { get; }

                public SelectPropertyNode SelectProperty { get; }
            }

            public sealed class Eighth : PropertyPath
            {
                public Eighth(SelectPropertyNode selectProperty)
                {
                    SelectProperty = selectProperty;
                }

                public SelectPropertyNode SelectProperty { get; }
            }

            public sealed class Ninth : PropertyPath
            {
                public Ninth(OdataUriNode.QualifiedComplexTypeName qualifiedComplexTypeName, OdataUriNode.QualifiedActionName qualifiedActionName)
                {
                    QualifiedComplexTypeName = qualifiedComplexTypeName;
                    QualifiedActionName = qualifiedActionName;
                }

                public OdataUriNode.QualifiedComplexTypeName QualifiedComplexTypeName { get; }

                public OdataUriNode.QualifiedActionName QualifiedActionName { get; }
            }

            public sealed class Tenth : PropertyPath
            {
                public Tenth(OdataUriNode.QualifiedActionName qualifiedActionName)
                {
                    QualifiedActionName = qualifiedActionName;
                }

                public OdataUriNode.QualifiedActionName QualifiedActionName { get; }
            }

            public sealed class Eleventh : PropertyPath
            {
                public Eleventh(OdataUriNode.QualifiedComplexTypeName qualifiedComplexTypeName, OdataUriNode.QualifiedFunctionName qualifiedFunctionName)
                {
                    QualifiedComplexTypeName = qualifiedComplexTypeName;

                    QualifiedFunctionName = qualifiedFunctionName;
                }

                public OdataUriNode.QualifiedComplexTypeName QualifiedComplexTypeName { get; }
                public OdataUriNode.QualifiedFunctionName QualifiedFunctionName { get; }
            }

            public sealed class Twelfth : PropertyPath
            {
                public Twelfth(OdataUriNode.QualifiedFunctionName qualifiedFunctionName)
                {
                    QualifiedFunctionName = qualifiedFunctionName;
                }

                public OdataUriNode.QualifiedFunctionName QualifiedFunctionName { get; }
            }
        }

        /// <summary>
        /// TODO i don't like this class name, but it was confliocting with <see cref="QualifiedEntityTypeName.Namespace"/> and <see cref="QualifiedComplexTypeName.Namespace"/>
        /// </summary>
        public abstract class SelectPropertyNode : SelectExpressionNode
        {
            private SelectPropertyNode()
            {
            }

            public sealed class PrimitiveProperty : SelectPropertyNode
            {
                public PrimitiveProperty(OdataUriNode.PrimitiveProperty property)
                {
                    Property = property;
                }

                public OdataUriNode.PrimitiveProperty Property { get; }
            }
        }
    }

    /// <summary>
    /// TODO move stuff out of here if it's not actually shared across different expression types
    /// </summary>
    public abstract class OdataUriNode
    {
        private OdataUriNode()
        {
        }

        /// <summary>
        /// TODO i don't like this class name, but it was confliocting with <see cref="SelectExpressionNode.PropertyPath.First.SelectProperty"/> and others
        /// </summary>
        public sealed class NamespaceNode : OdataUriNode
        {
            public NamespaceNode(IEnumerable<OdataIdentifier> namespaceParts)
            {
                NamespaceParts = namespaceParts;
            }

            public IEnumerable<OdataIdentifier> NamespaceParts { get; }
        }

        public sealed class OdataIdentifier : OdataUriNode
        {
            public OdataIdentifier(string identifier)
            {
                //// TODO use a regex match
                this.Identifier = identifier;
            }

            public string Identifier { get; }
        }

        public sealed class QualifiedEntityTypeName : OdataUriNode
        {
            public QualifiedEntityTypeName(NamespaceNode @namespace, OdataIdentifier entityTypeName)
            {
                Namespace = @namespace;

                EntityTypeName = entityTypeName;
            }

            public NamespaceNode Namespace { get; }

            public OdataIdentifier EntityTypeName { get; }
        }

        public sealed class QualifiedComplexTypeName : OdataUriNode
        {
            public QualifiedComplexTypeName(NamespaceNode @namespace, OdataIdentifier entityTypeName)
            {
                Namespace = @namespace;

                EntityTypeName = entityTypeName;
            }

            public NamespaceNode Namespace { get; }

            public OdataIdentifier EntityTypeName { get; }
        }

        public sealed class QualifiedActionName : OdataUriNode
        {
        }

        public sealed class QualifiedFunctionName : OdataUriNode
        {
        }

        public abstract class PrimitiveProperty : OdataUriNode
        {
            private PrimitiveProperty()
            {
            }

            public sealed class PrimitiveKeyProperty : PrimitiveProperty
            {
                public PrimitiveKeyProperty(OdataIdentifier identifier)
                {
                    Identifier = identifier;
                }

                public OdataIdentifier Identifier { get; }
            }

            public sealed class  PrimitiveNonKeyProperty : PrimitiveProperty
            {
                public PrimitiveNonKeyProperty(OdataIdentifier identifier)
                {
                    Identifier = identifier;
                }

                public OdataIdentifier Identifier { get; }
            }
        }
    }
}
