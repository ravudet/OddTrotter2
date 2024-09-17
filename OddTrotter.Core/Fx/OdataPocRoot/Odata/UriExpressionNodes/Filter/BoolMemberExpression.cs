///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

    public abstract class BoolMemberExpression
    {
        private BoolMemberExpression()
        {
        }

        public abstract class Qualified : BoolMemberExpression
        {
            private Qualified(QualifiedEntityTypeName qualifiedEntityTypeName)
            {
                QualifiedEntityTypeName = qualifiedEntityTypeName;
            }

            public QualifiedEntityTypeName QualifiedEntityTypeName { get; }

            public sealed class PropertyPath : Qualified
            {
                public PropertyPath(
                    QualifiedEntityTypeName qualifiedEntityTypeName,
                    BoolPropertyPathExpression boolPropertyPathExpression)
                    : base(qualifiedEntityTypeName)
                {
                    BoolPropertyPathExpression = boolPropertyPathExpression;
                }

                public BoolPropertyPathExpression BoolPropertyPathExpression { get; }
            }

            public sealed class BoundFunction : Qualified
            {
                public BoundFunction(
                    QualifiedEntityTypeName qualifiedEntityTypeName, 
                    FunctionExpression boundFunctionExpression)
                    : base(qualifiedEntityTypeName)
                {
                    BoundFunctionExpression = boundFunctionExpression;
                }

                public FunctionExpression BoundFunctionExpression { get; }
            }

            public sealed class BoolAnnotation : Qualified
            {
                public BoolAnnotation(
                    QualifiedEntityTypeName qualifiedEntityTypeName, 
                    BoolAnnotationExpression boolAnnotationExpression)
                    : base(qualifiedEntityTypeName)
                {
                    BoolAnnotationExpression = boolAnnotationExpression;
                }

                public BoolAnnotationExpression BoolAnnotationExpression { get; }
            }
        }

        public abstract class Unqualified : BoolMemberExpression
        {
            private Unqualified()
            {
            }


            public sealed class PropertyPath : Unqualified
            {
                public PropertyPath(BoolPropertyPathExpression boolPropertyPathExpression)
                {
                    BoolPropertyPathExpression = boolPropertyPathExpression;
                }

                public BoolPropertyPathExpression BoolPropertyPathExpression { get; }
            }

            public sealed class BoundFunction : Unqualified
            {
                public BoundFunction(FunctionExpression boundFunctionExpression)
                {
                    BoundFunctionExpression = boundFunctionExpression;
                }

                public FunctionExpression BoundFunctionExpression { get; }
            }

            public sealed class BoolAnnotation : Unqualified
            {
                public BoolAnnotation(BoolAnnotationExpression boolAnnotationExpression)
                {
                    BoolAnnotationExpression = boolAnnotationExpression;
                }

                public BoolAnnotationExpression BoolAnnotationExpression { get; }
            }
        }
    }
}
