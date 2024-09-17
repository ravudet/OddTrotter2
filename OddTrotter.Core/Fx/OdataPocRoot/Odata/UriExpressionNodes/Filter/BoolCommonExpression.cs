////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter
{
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

    //// TODO finish the rest of boolCommonExpr.abnf
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
            public Tenth()
            {
            }
        }

        /// <summary>
        /// TODO rename
        /// </summary>
        public sealed class Million : BoolCommonExpression
        {
            public Million(BoolCommonExpression boolCommonExpression, AndExpression andExpression)
            {
                BoolCommonExpression = boolCommonExpression;
                AndExpression = andExpression;
            }

            public BoolCommonExpression BoolCommonExpression { get; }

            public AndExpression AndExpression { get; }
        }

        //// TODO do other derived types after you've created abnf for boolcommonexpr
    }

    public sealed class AndExpression
    {
        public AndExpression(BoolCommonExpression right)
        {
            Right = right;
        }

        public BoolCommonExpression Right { get; }
    }
}