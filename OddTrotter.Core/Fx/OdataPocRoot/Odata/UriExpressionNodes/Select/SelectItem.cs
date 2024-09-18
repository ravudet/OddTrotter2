namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Select
{
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Diagnostics.Contracts;
    using System.Text;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

    public sealed class SelectItemStringBuilder : SelectItem.OtherVisitor<StringBuilder>
    {
        public override void SpecializedVisit(SelectItem.Star node, StringBuilder context)
        {
            context.Append("*");
        }
    }

    public sealed class SelectItemToString : SelectItem.Visitor<string>
    {
        public override string SpecializedVisit(SelectItem.Star node)
        {
            throw new System.NotImplementedException();
        }

        public override string SpecializedVisit(SelectItem.PropertyPath path)
        {
            return PropertyPathToString.Instance.Visit(path);
        }

        private sealed class PropertyPathToString : SelectItem.PropertyPath.Visitor<string>
        {
            private PropertyPathToString()
            {
            }

            public static PropertyPathToString Instance { get; } = new PropertyPathToString();

            public override string SpecializedVisit(SelectItem.PropertyPath.First node)
            {
                throw new System.NotImplementedException();
            }
        }
    }

    public abstract class SelectItem
    {
        private SelectItem()
        {
        }

        protected abstract T Accept<T>(Visitor<T> visitor);

        protected abstract void Accept<T>(OtherVisitor<T> visitor, T context);

        protected abstract TResult Accept<TResult, TContext>(OtherOtherVisitor<TResult, TContext> visitor, TContext context);

        public abstract class OtherOtherVisitor<TResult, TContext>
        {
            public TResult Visit(SelectItem node, TContext context)
            {
                return node.Accept(this, context);
            }
        }

        public abstract class Visitor<T>
        {
            public T Visit(SelectItem node)
            {
                return node.Accept(this);
            }

            public abstract T SpecializedVisit(Star node);

            //// TODO other methods

            public abstract T SpecializedVisit(PropertyPath path);
        }

        public abstract class OtherVisitor<T>
        {
            public void Visit(SelectItem node, T context)
            {
                node.Accept(this, context);
            }

            public abstract void SpecializedVisit(Star node, T context);
        }

        public sealed class Star : SelectItem
        {
            private Star()
            {
            }

            public static Star Instance { get; } = new Star();

            protected override T Accept<T>(Visitor<T> visitor)
            {
                return visitor.SpecializedVisit(this);
            }

            protected override void Accept<T>(OtherVisitor<T> visitor, T context)
            {
                return visitor.SpecializedVisit(this, context);
            }
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

            protected sealed override T Accept<T>(SelectItem.Visitor<T> visitor)
            {
                throw new System.NotImplementedException();
            }

            protected abstract T Accept<T>(Visitor<T> visitor);

            public new abstract class Visitor<T>
            {
                public T Visit(PropertyPath node)
                {
                    return node.Accept(this);
                }

                public abstract T SpecializedVisit(First node);
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
