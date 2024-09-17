////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

    public abstract class BoolCommonExpression
    {
        private BoolCommonExpression()
        {
        }

        public sealed class First : BoolCommonExpression
        {
            public First(BooleanValue booleanValue)
            {
                BooleanValue = booleanValue;
            }

            public BooleanValue BooleanValue { get; }
        }

        public sealed class Second : BoolCommonExpression
        {
            public Second(BoolRootExpression boolRootExpression)
            {
                BoolRootExpression = boolRootExpression;
            }

            public BoolRootExpression BoolRootExpression { get; }
        }

        public sealed class Third : BoolCommonExpression
        {
            public Third(BoolFirstMemberExpression boolFirstMemberExpression)
            {
                BoolFirstMemberExpression = boolFirstMemberExpression;
            }

            public BoolFirstMemberExpression BoolFirstMemberExpression { get; }
        }

        public sealed class Fourth : BoolCommonExpression
        {
            public Fourth(BoolFunctionExpression boolFunctionExpression)
            {
                BoolFunctionExpression = boolFunctionExpression;
            }

            public BoolFunctionExpression BoolFunctionExpression { get; }
        }

        public sealed class Fifth : BoolCommonExpression
        {
            public Fifth(BoolMethodCallExpression boolMethodCallExpression)
            {
                BoolMethodCallExpression = boolMethodCallExpression;
            }

            public BoolMethodCallExpression BoolMethodCallExpression { get; }
        }

        public sealed class Sixth : BoolCommonExpression
        {
            public Sixth(BoolParenExpression boolParenExpression)
            {
                BoolParenExpression = boolParenExpression;
            }

            public BoolParenExpression BoolParenExpression { get; }
        }

        public sealed class Seventh : BoolCommonExpression
        {
            private Seventh()
            {
                throw new System.Exception("TODO");
            }
        }

        public sealed class Eighth : BoolCommonExpression
        {
            public Eighth(IsofExpression isofExpression)
            {
                IsofExpression = isofExpression;
            }

            public IsofExpression IsofExpression { get; }
        }

        public sealed class Ninth : BoolCommonExpression
        {
            public Ninth(NotExpression notExpression)
            {
                NotExpression = notExpression;
            }

            public NotExpression NotExpression { get; }
        }

        public sealed class Tenth : BoolCommonExpression
        {
            public Tenth(PrimitiveLiteral primitiveLiteral, EqualsExpression equalsExpression)
            {
                PrimitiveLiteral = primitiveLiteral;
                EqualsExpression = equalsExpression;
            }

            public PrimitiveLiteral PrimitiveLiteral { get; }

            public EqualsExpression EqualsExpression { get; }
        }

        public sealed class Eleventh : BoolCommonExpression
        {
            public Eleventh(PrimitiveLiteral primitiveLiteral, NotEqualsExpression notEqualsExpression)
            {
                PrimitiveLiteral = primitiveLiteral;
                NotEqualsExpression = notEqualsExpression;
            }

            public PrimitiveLiteral PrimitiveLiteral { get; }

            public NotEqualsExpression NotEqualsExpression { get; }
        }

        public sealed class Twelfth : BoolCommonExpression
        {
            public Twelfth(PrimitiveLiteral primitiveLiteral, LessThanExpression lessThanExpression)
            {
                PrimitiveLiteral = primitiveLiteral;
                LessThanExpression = lessThanExpression;
            }

            public PrimitiveLiteral PrimitiveLiteral { get; }

            public LessThanExpression LessThanExpression { get; }
        }

        public sealed class Thirteenth : BoolCommonExpression
        {
            public Thirteenth(
                PrimitiveLiteral primitiveLiteral,
                LessThanOrEqualToExpression lessThanOrEqualToExpression)
            {
                PrimitiveLiteral = primitiveLiteral;
                LessThanOrEqualToExpression = lessThanOrEqualToExpression;
            }

            public PrimitiveLiteral PrimitiveLiteral { get; }

            public LessThanOrEqualToExpression LessThanOrEqualToExpression { get; }
        }

        public sealed class Fourteenth : BoolCommonExpression
        {
            public Fourteenth(PrimitiveLiteral primitiveLiteral, GreaterThanExpression greaterThanExpression)
            {
                PrimitiveLiteral = primitiveLiteral;
                GreaterThanExpression = greaterThanExpression;
            }

            public PrimitiveLiteral PrimitiveLiteral { get; }

            public GreaterThanExpression GreaterThanExpression { get; }
        }

        public sealed class Fifteenth : BoolCommonExpression
        {
            public Fifteenth(
                PrimitiveLiteral primitiveLiteral,
                GreatherThanOrEqualToExpression greatherThanOrEqualToExpression)
            {
                PrimitiveLiteral = primitiveLiteral;
                GreatherThanOrEqualToExpression = greatherThanOrEqualToExpression;
            }

            public PrimitiveLiteral PrimitiveLiteral { get; }

            public GreatherThanOrEqualToExpression GreatherThanOrEqualToExpression { get; }
        }

        public sealed class Sixteenth : BoolCommonExpression
        {
            public Sixteenth(PrimitiveLiteral primitiveLiteral, HasExpression hasExpression)
            {
                PrimitiveLiteral = primitiveLiteral;
                HasExpression = hasExpression;
            }

            public PrimitiveLiteral PrimitiveLiteral { get; }

            public HasExpression HasExpression { get; }
        }

        public sealed class Seventeenth : BoolCommonExpression
        {
            public Seventeenth(PrimitiveLiteral primitiveLiteral, InExpression inExpression)
            {
                PrimitiveLiteral = primitiveLiteral;
                InExpression = inExpression;
            }

            public PrimitiveLiteral PrimitiveLiteral { get; }

            public InExpression InExpression { get; }
        }

        //// TODO do other derived types from boolCommonExpr.abnf

        public sealed class Eighteenth : BoolCommonExpression
        {
            public Eighteenth(BoolCommonExpression boolCommonExpression, AndExpression andExpression)
            {
                //// TODO the left side is not actually boolcommonexpression; check the boolCommonExpr.abnf once it's
                //// finalized
                BoolCommonExpression = boolCommonExpression;
                AndExpression = andExpression;
            }

            public BoolCommonExpression BoolCommonExpression { get; }

            public AndExpression AndExpression { get; }
        }

        public sealed class Nineteenth : BoolCommonExpression
        {
            public Nineteenth(BoolCommonExpression boolCommonExpression, OrExpression orExpression)
            {
                //// TODO the left side is not actually boolcommonexpression; check the boolCommonExpr.abnf once it's
                //// finalized
                BoolCommonExpression = boolCommonExpression;
                OrExpression = orExpression;
            }

            public BoolCommonExpression BoolCommonExpression { get; }

            public OrExpression OrExpression { get; }
        }

        //// TODO do other derived types after you've finished boolCommonExpr.abnf
    }
}