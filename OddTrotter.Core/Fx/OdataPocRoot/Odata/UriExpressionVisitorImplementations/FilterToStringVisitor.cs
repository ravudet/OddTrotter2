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
            else if (node is BoolCommonExpression.Third third)
            {
                Visit(third.BoolFirstMemberExpression, builder);
            }
            else if (node is BoolCommonExpression.Eighteenth million)
            {
                Visit(million.BoolCommonExpression, builder);
                Visit(million.AndExpression, builder);
            }
            else
            {
                throw new Exception("TODO a proper visitor pattern would prevent this branch");
            }
        }

        public void Visit(BoolFirstMemberExpression node, StringBuilder builder)
        {
            if (node is BoolFirstMemberExpression.BoolMemberExpressionNode boolMemberExpressionNode)
            {
                Visit(boolMemberExpressionNode.BoolMemberExpression, builder);
            }
            else
            {
                throw new Exception("TODO a proper visitor pattern would prevent this branch");
            }
        }

        public void Visit(BoolMemberExpression node, StringBuilder builder)
        {
            if (node is BoolMemberExpression.Unqualified.PropertyPath unqualifiedPropertyPath)
            {
                Visit(unqualifiedPropertyPath.BoolPropertyPathExpression, builder);
            }
            else
            {
                throw new Exception("TODO a proper visitor pattern would prevent this branch");
            }
        }

        public void Visit(BoolPropertyPathExpression node, StringBuilder builder)
        {
            if (node is BoolPropertyPathExpression.PrimitveNode.Primitive primitiveNodePrimitive)
            {
                Visit(primitiveNodePrimitive.PrimitiveProperty, builder);
            }
            else if (node is BoolPropertyPathExpression.Entity entity)
            {
                //// TODO constructor injection
                CommonToStringVisitor.Default.Visit(entity.EntityNavigationProperty, builder);
                builder.Append("/");
                Visit(entity.BoolSingleNavigationExpression, builder);
            }
            else
            {
                throw new Exception("TODO a proper visitor pattern would prevent this branch");
            }
        }

        public void Visit(BoolSingleNavigationExpression node, StringBuilder builder)
        {
            Visit(node.BoolMemberExpression, builder);
        }

        public void Visit(PrimitiveProperty node, StringBuilder builder)
        {
            //// TODO move this to common visitor
            if (node is PrimitiveProperty.PrimitiveNonKeyProperty nonKeyProperty)
            {
                //// TODO constructor injection
                CommonToStringVisitor.Default.Visit(nonKeyProperty.Identifier, builder);
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
