////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.Visitors
{
    using System;
    using global::System.Text;

    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter;

    public sealed class CommonExpressionPart1ToStringVisitor : CommonExpressionPart1.Visitor<Void, StringBuilder>
    {
        private readonly CommonExpressionToStringVisitor commonExpressionToStringVisitor;

        private readonly PrimitiveLiteralToStringVisitor primitiveLiteralToStringVisitor;
        private readonly ArrayOrObjectToStringVisitor arrayOrObjectToStringVisitor;
        private readonly RootExprToStringVisitor rootExprToStringVisitor;
        private readonly FirstMemberExprToStringVisitor firstMemberExprToStringVisitor;
        private readonly FunctionExprToStringVisitor functionExprToStringVisitor;
        private readonly NegateExprToStringVisitor negateExprToStringVisitor;
        private readonly MethodCallExprToStringVisitor methodCallExprToStringVisitor;
        private readonly ListExprToStringVisitor listExprToStringVisitor;
        private readonly CastExprToStringVisitor castExprToStringVisitor;
        private readonly IsofExprToStringVisitor isofExprToStringVisitor;
        private readonly NotExprToStringVisitor notExprToStringVisitor;

        public CommonExpressionPart1ToStringVisitor(
            CommonExpressionToStringVisitor commonExpressionToStringVisitor,
            PrimitiveLiteralToStringVisitor primitiveLiteralToStringVisitor,
            ArrayOrObjectToStringVisitor arrayOrObjectToStringVisitor,
            RootExprToStringVisitor rootExprToStringVisitor,
            FirstMemberExprToStringVisitor firstMemberExprToStringVisitor,
            FunctionExprToStringVisitor functionExprToStringVisitor,
            NegateExprToStringVisitor negateExprToStringVisitor,
            MethodCallExprToStringVisitor methodCallExprToStringVisitor,
            ListExprToStringVisitor listExprToStringVisitor,
            CastExprToStringVisitor castExprToStringVisitor,
            IsofExprToStringVisitor isofExprToStringVisitor,
            NotExprToStringVisitor notExprToStringVisitor)
        {
            this.commonExpressionToStringVisitor = commonExpressionToStringVisitor;

            this.primitiveLiteralToStringVisitor = primitiveLiteralToStringVisitor;
            this.arrayOrObjectToStringVisitor = arrayOrObjectToStringVisitor;
            this.rootExprToStringVisitor = rootExprToStringVisitor;
            this.firstMemberExprToStringVisitor = firstMemberExprToStringVisitor;
            this.functionExprToStringVisitor = functionExprToStringVisitor;
            this.negateExprToStringVisitor = negateExprToStringVisitor;
            this.methodCallExprToStringVisitor = methodCallExprToStringVisitor;
            this.listExprToStringVisitor = listExprToStringVisitor;
            this.castExprToStringVisitor = castExprToStringVisitor;
            this.isofExprToStringVisitor = isofExprToStringVisitor;
            this.notExprToStringVisitor = notExprToStringVisitor;
        }

        public sealed override Void Visit(CommonExpressionPart1.PrimitiveLiteral node, StringBuilder context)
        {
            return primitiveLiteralToStringVisitor.Traverse(
                node.Value,
                context);
        }

        public sealed override Void Visit(CommonExpressionPart1.ArrayOrObject node, StringBuilder context)
        {
            return arrayOrObjectToStringVisitor.Traverse(
                node.Value,
                context);
        }

        public sealed override Void Visit(CommonExpressionPart1.RootExpr node, StringBuilder context)
        {
            return rootExprToStringVisitor.Traverse(
                node.Value,
                context);
        }

        public sealed override Void Visit(CommonExpressionPart1.FirstMemberExpr node, StringBuilder context)
        {
            return firstMemberExprToStringVisitor.Traverse(
                node.Value,
                context);
        }

        public sealed override Void Visit(CommonExpressionPart1.FunctionExpr node, StringBuilder context)
        {
            return functionExprToStringVisitor.Traverse(
                node.Value,
                context);
        }

        public sealed override Void Visit(CommonExpressionPart1.NegateExpr node, StringBuilder context)
        {
            return negateExprToStringVisitor.Traverse(
                node.Value,
                context);
        }

        public sealed override Void Visit(CommonExpressionPart1.MethodCallExpr node, StringBuilder context)
        {
            return methodCallExprToStringVisitor.Traverse(
                node.Value,
                context);
        }

        public sealed override Void Visit(CommonExpressionPart1.ParenExpr node, StringBuilder context)
        {
            context.Append("(");
            commonExpressionToStringVisitor.Traverse(node.Value.CommonExpression, context);
            context.Append(")");

            return default;
        }

        public sealed override Void Visit(CommonExpressionPart1.ListExpr node, StringBuilder context)
        {
            return listExprToStringVisitor.Traverse(
                node.Value,
                context);
        }

        public sealed override Void Visit(CommonExpressionPart1.CastExpr node, StringBuilder context)
        {
            return castExprToStringVisitor.Traverse(
                node.Value,
                context);
        }

        public sealed override Void Visit(CommonExpressionPart1.IsofExpr node, StringBuilder context)
        {
            return isofExprToStringVisitor.Traverse(
                node.Value,
                context);
        }

        public sealed override Void Visit(CommonExpressionPart1.NotExpr node, StringBuilder context)
        {
            return notExprToStringVisitor.Traverse(
                node.Value,
                context);
        }
    }
}
