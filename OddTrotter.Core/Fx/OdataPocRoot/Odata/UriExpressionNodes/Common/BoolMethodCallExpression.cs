////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Common
{
    public abstract class BoolMethodCallExpression
    {
        private BoolMethodCallExpression()
        {
        }

        public sealed class EndsWith : BoolMethodCallExpression
        {
            public EndsWith(EndsWithMethodCallExpression endsWithMethodCallExpression)
            {
                EndsWithMethodCallExpression = endsWithMethodCallExpression;
            }

            public EndsWithMethodCallExpression EndsWithMethodCallExpression { get; }
        }

        public sealed class StartsWith : BoolMethodCallExpression
        {
            public StartsWith(StartsWithMethodCallExpression startsWithMethodCallExpression)
            {
                StartsWithMethodCallExpression = startsWithMethodCallExpression;
            }

            public StartsWithMethodCallExpression StartsWithMethodCallExpression { get; }
        }

        public sealed class Contains : BoolMethodCallExpression
        {
            public Contains(ContainsMethodCallExpression containsMethodCallExpression)
            {
                ContainsMethodCallExpression = containsMethodCallExpression;
            }

            public ContainsMethodCallExpression ContainsMethodCallExpression { get; }
        }

        public sealed class Intersects : BoolMethodCallExpression
        {
            public Intersects(IntersectsMethodCallExpression intersectsMethodCallExpression)
            {
                IntersectsMethodCallExpression = intersectsMethodCallExpression;
            }

            public IntersectsMethodCallExpression IntersectsMethodCallExpression { get; }
        }

        public sealed class HasSubset : BoolMethodCallExpression
        {
            public HasSubset(HasSubsetMethodCallExpression hasSubsetMethodCallExpression)
            {
                HasSubsetMethodCallExpression = hasSubsetMethodCallExpression;
            }

            public HasSubsetMethodCallExpression HasSubsetMethodCallExpression { get; }
        }

        public sealed class HasSubsequence : BoolMethodCallExpression
        {
            public HasSubsequence(HasSubsequenceMethodCallExpression hasSubsequenceMethodCallExpression)
            {
                HasSubsequenceMethodCallExpression = hasSubsequenceMethodCallExpression;
            }

            public HasSubsequenceMethodCallExpression HasSubsequenceMethodCallExpression { get; }
        }
    }
}
