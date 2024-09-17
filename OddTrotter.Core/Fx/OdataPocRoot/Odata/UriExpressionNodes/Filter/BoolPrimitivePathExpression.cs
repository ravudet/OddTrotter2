///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

    public abstract class BoolPrimitivePathExpression
    {
        private BoolPrimitivePathExpression()
        {
        }

        public sealed class Annotation : BoolPrimitivePathExpression
        {
            public Annotation(BoolAnnotationExpression boolAnnotationExpression)
            {
                BoolAnnotationExpression = boolAnnotationExpression;
            }

            public BoolAnnotationExpression BoolAnnotationExpression { get; }
        }

        public sealed class BoundFunction : BoolPrimitivePathExpression
        {
            public BoundFunction(FunctionExpression boundFunctionExpression)
            {
                BoundFunctionExpression = boundFunctionExpression;
            }

            public FunctionExpression BoundFunctionExpression { get; }
        }
    }
}
