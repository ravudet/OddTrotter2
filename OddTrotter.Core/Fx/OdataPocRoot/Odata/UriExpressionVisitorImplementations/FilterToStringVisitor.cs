namespace Fx.OdataPocRoot.Odata.UriExpressionVisitorImplementations
{
    using System;
    using System.Text;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter;

    public sealed class FilterToStringVisitor
    {
        public void Visit(Filter node, StringBuilder builder)
        {
            builder.Append("$filter=");
            Visit(node.BoolCommonExpression, builder);
        }

        public void Visit(BoolCommonExpression node, StringBuilder builder)
        {
            if (node is BoolCommonExpression.First first)
            {
                Visit(first.BooleanValue, builder);
            }
            else if (node is BoolCommonExpression.Million million)
            {
                Visit(million.BoolCommonExpression, builder);
                Visit(million.AndExpression, builder);
            }
            else
            {
                throw new Exception("TODO a proper visitor pattern would prevent this branch");
            }
        }

        public void Visit(BooleanValue node, StringBuilder builder)
        {
            if (node is BooleanValue.True)
            {
                builder.Append("true");
            }
            else if (node is BooleanValue.False)
            {
                builder.Append("false");
            }
            else
            {
                throw new Exception("TODO a proper visitor pattern would prevent this branch");
            }
        }

        public void Visit(AndExpression node, StringBuilder builder)
        {
            builder.Append(" and ");
            Visit(node.Right, builder);
        }
    }
}
