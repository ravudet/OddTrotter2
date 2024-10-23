////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter.Visitors
{
    using System;
    using global::System.Text;

    using Fx.OdataPocRoot.V2.Odata.UriQueryOptions.Filter;

    public sealed class CommonExpressionPart4ToStringVisitor : CommonExpressionPart4.Visitor<Void, StringBuilder>
    {
        private readonly CommonExpressionToStringVisitor commonExpressionToStringVisitor;

        public CommonExpressionPart4ToStringVisitor(CommonExpressionToStringVisitor commonExpressionToStringVisitor)
        {
            this.commonExpressionToStringVisitor = commonExpressionToStringVisitor;
        }

        public override Void Visit(CommonExpressionPart4.AndExpr node, StringBuilder context)
        {
            context.Append(" and "); //// TODO make this configurable?
            commonExpressionToStringVisitor.Traverse(
                node.BoolCommonExpression.CommonExpression,
                context);

            return default;
        }

        public override Void Visit(CommonExpressionPart4.OrExpr node, StringBuilder context)
        {
            context.Append(" or "); //// TODO make this configurable?
            commonExpressionToStringVisitor.Traverse(
                node.BoolCommonExpression.CommonExpression,
                context);

            return default;
        }
    }
}
