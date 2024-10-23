////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.ComponentModel;

namespace Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter
{
    public abstract class CommonExpressionPart1
    {
        //// TODO should these "parts" be nested in CommonExpression?

        private CommonExpressionPart1()
        {
        }

        public sealed class PrimitiveLiteral : CommonExpressionPart1
        {
            private PrimitiveLiteral()
            {
                //// TODO do this before "shipping", but leave the actual primitive literal node stubbed
            }
        }

        public sealed class ArrayOrObject : CommonExpressionPart1
        {
            private ArrayOrObject()
            {
                //// TODO do this before "shipping", but leave the actual primitive literal node stubbed
            }
        }

        public sealed class RootExpr : CommonExpressionPart1
        {
            private RootExpr()
            {
                //// TODO do this before "shipping", but leave the actual primitive literal node stubbed
            }
        }

        public sealed class FirstMemberExpr : CommonExpressionPart1
        {
            private FirstMemberExpr()
            {
                //// TODO do this before "shipping", but leave the actual primitive literal node stubbed
            }
        }

        public sealed class FunctionExpr : CommonExpressionPart1
        {
            private FunctionExpr()
            {
                //// TODO do this before "shipping", but leave the actual primitive literal node stubbed
            }
        }

        public sealed class NegateExpr : CommonExpressionPart1
        {
            private NegateExpr()
            {
                //// TODO do this before "shipping", but leave the actual primitive literal node stubbed
            }
        }

        public sealed class MethodCallExpr : CommonExpressionPart1
        {
            private MethodCallExpr()
            {
                //// TODO do this before "shipping", but leave the actual primitive literal node stubbed
            }
        }

        public sealed class ParenExpr : CommonExpressionPart1
        {
            public ParenExpr(CommonExpression commonExpression)
            {
                this.CommonExpression = commonExpression;
            }

            public CommonExpression CommonExpression { get; }
        }


        //// TODO do the rest before "shipping", but leave the actual primitive literal node stubbed
    }

    public abstract class CommonExpressionPart2
    {
        private CommonExpressionPart2()
        {
        }
    }

    public abstract class CommonExpressionPart3
    {
        private CommonExpressionPart3()
        {
        }
    }

    public abstract class CommonExpressionPart4
    {
        private CommonExpressionPart4()
        {
        }

        public sealed class AndExpr : CommonExpressionPart4
        {
            public AndExpr(BoolCommonExpression boolCommonExpression)
            {
                this.BoolCommonExpression = boolCommonExpression;
            }

            public BoolCommonExpression BoolCommonExpression { get; }
        }

        public sealed class OrExpr : CommonExpressionPart4
        {
            public OrExpr(BoolCommonExpression boolCommonExpression)
            {
                this.BoolCommonExpression = boolCommonExpression;
            }

            public BoolCommonExpression BoolCommonExpression { get; }
        }
    }

    public abstract class CommonExpression
    {
        private CommonExpression()
        {
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Traverse(CommonExpression node, TContext context)
            {
                return node.Accept(this, context);
            }

            public abstract TResult Visit(Part1Only node, TContext context); //// TODO any way to make these protected?

            public abstract TResult Visit(Part1Part2 node, TContext context);

            public abstract TResult Visit(Part1Part2Part3 node, TContext context);

            public abstract TResult Visit(Part1Part2Part3Part4 node, TContext context);

            public abstract TResult Visit(Part1Part2Part4 node, TContext context);

            public abstract TResult Visit(Part1Part3 node, TContext context);

            public abstract TResult Visit(Part1Part3Part4 node, TContext context);

            public abstract TResult Visit(Part1Part4 node, TContext context);
        }

        public sealed class Part1Only : CommonExpression
        {
            public Part1Only(CommonExpressionPart1 part1)
            {
                this.Part1 = part1;
            }

            public CommonExpressionPart1 Part1 { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class Part1Part2 : CommonExpression
        {
            public Part1Part2(CommonExpressionPart1 part1, CommonExpressionPart2 part2)
            {
                this.Part1 = part1;
                this.Part2 = part2;
            }

            public CommonExpressionPart1 Part1 { get; }

            public CommonExpressionPart2 Part2 { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class Part1Part2Part3 : CommonExpression
        {
            public Part1Part2Part3(CommonExpressionPart1 part1, CommonExpressionPart2 part2, CommonExpressionPart3 part3)
            {
                this.Part1 = part1;
                this.Part2 = part2;
                this.Part3 = part3;
            }

            public CommonExpressionPart1 Part1 { get; }

            public CommonExpressionPart2 Part2 { get; }

            public CommonExpressionPart3 Part3 { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class Part1Part2Part3Part4 : CommonExpression
        {
            public Part1Part2Part3Part4(CommonExpressionPart1 part1, CommonExpressionPart2 part2, CommonExpressionPart3 part3, CommonExpressionPart4 part4)
            {
                this.Part1 = part1;
                this.Part2 = part2;
                this.Part3 = part3;
                this.Part4 = part4;
            }

            public CommonExpressionPart1 Part1 { get; }

            public CommonExpressionPart2 Part2 { get; }

            public CommonExpressionPart3 Part3 { get; }

            public CommonExpressionPart4 Part4 { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class Part1Part2Part4 : CommonExpression
        {
            public Part1Part2Part4(CommonExpressionPart1 part1, CommonExpressionPart2 part2, CommonExpressionPart4 part4)
            {
                this.Part1 = part1;
                this.Part2 = part2;
                this.Part4 = part4;
            }

            public CommonExpressionPart1 Part1 { get; }

            public CommonExpressionPart2 Part2 { get; }

            public CommonExpressionPart4 Part4 { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class Part1Part3 : CommonExpression
        {
            public Part1Part3(CommonExpressionPart1 part1, CommonExpressionPart3 part3)
            {
                this.Part1 = part1;
                this.Part3 = part3;
            }

            public CommonExpressionPart1 Part1 { get; }

            public CommonExpressionPart3 Part3 { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class Part1Part3Part4 : CommonExpression
        {
            public Part1Part3Part4(CommonExpressionPart1 part1, CommonExpressionPart3 part3, CommonExpressionPart4 part4)
            {
                this.Part1 = part1;
                this.Part3 = part3;
                this.Part4 = part4;
            }

            public CommonExpressionPart1 Part1 { get; }

            public CommonExpressionPart3 Part3 { get; }

            public CommonExpressionPart4 Part4 { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class Part1Part4 : CommonExpression
        {
            public Part1Part4(CommonExpressionPart1 part1, CommonExpressionPart4 part4)
            {
                this.Part1 = part1;
                this.Part4 = part4;
            }

            public CommonExpressionPart1 Part1 { get; }

            public CommonExpressionPart4 Part4 { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }
    }
}
