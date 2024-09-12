namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Select
{
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

                public First(QualifiedEntityTypeName qualifiedEntityTypeName, SelectPropertyNode selectProperty)
                {
                    QualifiedEntityTypeName = qualifiedEntityTypeName;
                    SelectProperty = selectProperty;
                }

                public QualifiedEntityTypeName QualifiedEntityTypeName { get; }

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
                public Third(QualifiedEntityTypeName qualifiedEntityTypeName, OdataUriNode.QualifiedActionName qualifiedActionName)
                {
                    QualifiedEntityTypeName = qualifiedEntityTypeName;
                    QualifiedActionName = qualifiedActionName;
                }

                public QualifiedEntityTypeName QualifiedEntityTypeName { get; }

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
                public Fifth(QualifiedEntityTypeName qualifiedEntityTypeName, OdataUriNode.QualifiedFunctionName qualifiedFunctionName)
                {
                    QualifiedEntityTypeName = qualifiedEntityTypeName;
                    QualifiedFunctionName = qualifiedFunctionName;
                }

                public QualifiedEntityTypeName QualifiedEntityTypeName { get; }

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
                public Seventh(QualifiedComplexTypeName qualifiedComplexTypeName, SelectPropertyNode selectProperty)
                {
                    QualifiedComplexTypeName = qualifiedComplexTypeName;
                    SelectProperty = selectProperty;
                }

                public QualifiedComplexTypeName QualifiedComplexTypeName { get; }

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
                public Ninth(QualifiedComplexTypeName qualifiedComplexTypeName, OdataUriNode.QualifiedActionName qualifiedActionName)
                {
                    QualifiedComplexTypeName = qualifiedComplexTypeName;
                    QualifiedActionName = qualifiedActionName;
                }

                public QualifiedComplexTypeName QualifiedComplexTypeName { get; }

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
                public Eleventh(QualifiedComplexTypeName qualifiedComplexTypeName, OdataUriNode.QualifiedFunctionName qualifiedFunctionName)
                {
                    QualifiedComplexTypeName = qualifiedComplexTypeName;

                    QualifiedFunctionName = qualifiedFunctionName;
                }

                public QualifiedComplexTypeName QualifiedComplexTypeName { get; }

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
        public abstract class SelectPropertyNode : SelectItem
        {
            private SelectPropertyNode()
            {
            }

            public sealed class PrimitiveProperty : SelectPropertyNode
            {
                public PrimitiveProperty(PrimitiveProperty property)
                {
                    Property = property;
                }

                public PrimitiveProperty Property { get; }
            }

            public sealed class PrimitiveCollectionProperty : SelectPropertyNode
            {
                public PrimitiveCollectionProperty(OdataIdentifier property)
                    : this(property, null)
                {
                }

                public PrimitiveCollectionProperty(OdataIdentifier property, FilterExpressionNode? filterExpressionNode) //// TODO add the other expression node types from selectoptionpc in the ABNF; do you want to use derived types for this?
                {
                    //// TODO this is where the spaghetti occurs; you now have to pull in all of the required nodes for the rest of the query options
                    Property = property;
                    FilterExpressionNode = filterExpressionNode;
                }

                public OdataIdentifier Property { get; }

                public FilterExpressionNode? FilterExpressionNode { get; }
            }

            public sealed class NavigationProperty : SelectPropertyNode
            {
                public NavigationProperty(NavigationProperty property)
                {
                    Property = property;
                }

                public NavigationProperty Property { get; }
            }

            public abstract class FullSelectPath : SelectPropertyNode
            {
                private FullSelectPath()
                {
                }
            }
        }
    }
}
