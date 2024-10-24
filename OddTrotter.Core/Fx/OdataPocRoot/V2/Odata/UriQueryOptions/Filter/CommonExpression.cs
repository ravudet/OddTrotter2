﻿////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter
{
    public abstract class CommonExpressionPart1
    {
        //// TODO should these "parts" be nested in CommonExpression? ANSWER yes, nest these

        private CommonExpressionPart1()
        {
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Traverse(CommonExpressionPart1 node, TContext context)
            {
                return node.Accept(this, context);
            }

            public abstract TResult Visit(PrimitiveLiteral node, TContext context); //// TODO any way to make these protected?

            public abstract TResult Visit(ArrayOrObject node, TContext context);

            public abstract TResult Visit(RootExpr node, TContext context);

            public abstract TResult Visit(FirstMemberExpr node, TContext context);

            public abstract TResult Visit(FunctionExpr node, TContext context);

            public abstract TResult Visit(NegateExpr node, TContext context);

            public abstract TResult Visit(MethodCallExpr node, TContext context);

            public abstract TResult Visit(ParenExpr node, TContext context);

            public abstract TResult Visit(ListExpr node, TContext context);

            public abstract TResult Visit(CastExpr node, TContext context);

            public abstract TResult Visit(IsofExpr node, TContext context);

            public abstract TResult Visit(NotExpr node, TContext context);
        }

        public sealed class PrimitiveLiteral : CommonExpressionPart1
        {
            public PrimitiveLiteral(Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.PrimitiveLiteral primitiveLiteral)
            {
                this.Value = primitiveLiteral;
            }

            public Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.PrimitiveLiteral Value { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class ArrayOrObject : CommonExpressionPart1
        {
            public ArrayOrObject(Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.ArrayOrObject arrayOrObject)
            {
                this.Value = arrayOrObject;
            }

            public Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.ArrayOrObject Value { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class RootExpr : CommonExpressionPart1
        {
            public RootExpr(Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.RootExpr rootExpr)
            {
                this.Value = rootExpr;
            }

            public Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.RootExpr Value { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class FirstMemberExpr : CommonExpressionPart1
        {
            public FirstMemberExpr(Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.FirstMemberExpr firstMemberExpr)
            {
                this.Value = firstMemberExpr;
            }

            public Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.FirstMemberExpr Value { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class FunctionExpr : CommonExpressionPart1
        {
            public FunctionExpr(Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.FunctionExpr functionExpr)
            {
                this.Value = functionExpr;
            }

            public Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.FunctionExpr Value { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class NegateExpr : CommonExpressionPart1
        {
            public NegateExpr(Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.NegateExpr negateExpr)
            {
                this.Value = negateExpr;
            }

            public Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.NegateExpr Value { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class MethodCallExpr : CommonExpressionPart1
        {
            public MethodCallExpr(Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.MethodCallExpr methodCallExpr)
            {
                this.Value = methodCallExpr;
            }

            public Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.MethodCallExpr Value { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class ParenExpr : CommonExpressionPart1
        {
            public ParenExpr(Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.ParenExpr parenExpr)
            {
                this.Value = parenExpr;
            }

            public Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.ParenExpr Value { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class ListExpr : CommonExpressionPart1
        {
            public ListExpr(Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.ListExpr listExpr)
            {
                this.Value = listExpr;
            }

            public Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.ListExpr Value { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class CastExpr : CommonExpressionPart1
        {
            public CastExpr(Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.CastExpr castExpr)
            {
                this.Value = castExpr;
            }

            public Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.CastExpr Value { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class IsofExpr : CommonExpressionPart1
        {
            public IsofExpr(Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.IsofExpr isofExpr)
            {
                this.Value = isofExpr;
            }

            public Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.IsofExpr Value { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class NotExpr : CommonExpressionPart1
        {
            public NotExpr(Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.NotExpr notExpr)
            {
                this.Value = notExpr;
            }

            public Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.NotExpr Value { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }
    }

    public abstract class CommonExpressionPart2
    {
        private CommonExpressionPart2()
        {
            //// TODO FEATURE GAP: finish this
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Traverse(CommonExpressionPart2 node, TContext context)
            {
                return node.Accept(this, context);
            }

            public abstract TResult Visit(Variant1 node, TContext context);
        }

        public sealed class Variant1 : CommonExpressionPart2
        {
            public Variant1(AddExpr addExpr)
            {
                this.AddExpr = addExpr;
            }

            public AddExpr AddExpr { get; }

            protected override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }
    }

    public abstract class CommonExpressionPart3
    {
        private CommonExpressionPart3()
        {
            //// TODO FEATURE GAP: finish this
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Traverse(CommonExpressionPart3 node, TContext context)
            {
                return node.Accept(this, context);
            }
        }
    }

    public abstract class CommonExpressionPart4
    {
        private CommonExpressionPart4()
        {
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Traverse(CommonExpressionPart4 node, TContext context)
            {
                return node.Accept(this, context);
            }

            public abstract TResult Visit(AndExpr node, TContext context); //// TODO any way to make these protected?

            public abstract TResult Visit(OrExpr node, TContext context);
        }

        public sealed class AndExpr : CommonExpressionPart4
        {
            public AndExpr(BoolCommonExpression boolCommonExpression)
            {
                this.BoolCommonExpression = boolCommonExpression;
            }

            public BoolCommonExpression BoolCommonExpression { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class OrExpr : CommonExpressionPart4
        {
            public OrExpr(BoolCommonExpression boolCommonExpression)
            {
                this.BoolCommonExpression = boolCommonExpression;
            }

            public BoolCommonExpression BoolCommonExpression { get; }

            protected sealed override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
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
                //// TODO maybe call this switch?
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

    public abstract class AUnion
    {
        private AUnion()
        {
        }

        protected abstract TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context);

        public abstract class Visitor<TResult, TContext>
        {
            public TResult Traverse(AUnion node, TContext context)
            {
                return node.Accept(this, context);
            }

            public abstract TResult Visit(FirstKind node, TContext context);

            public abstract TResult Visit(SecondKind node, TContext context);
        }

        public sealed class FirstKind : AUnion
        {
            public FirstKind()
            {
            }

            protected override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }

        public sealed class SecondKind : AUnion
        {
            public SecondKind()
            {
            }

            protected override TResult Accept<TResult, TContext>(Visitor<TResult, TContext> visitor, TContext context)
            {
                return visitor.Visit(this, context);
            }
        }
    }
}
