namespace Fx.OdataPocRoot.Odata.UriExpressionVisitorImplementations
{
    using System;
    using System.Linq;
    using System.Text;

    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

    public sealed class CommonToStringVisitor
    {
        private CommonToStringVisitor()
        {
        }

        public static CommonToStringVisitor Default { get; } = new CommonToStringVisitor();

        public void Visit(Namespace node, StringBuilder builder)
        {
            //// TODO should the node have a readonlylist of namespace parts?
            var namespaceParts = node.NamespaceParts.ToList();
            if (namespaceParts.Count == 0)
            {
                //// TODO is this actually legal? model it somehow if it's not
                return;
            }

            Visit(namespaceParts[0], builder);
            for (int i = 1; i < namespaceParts.Count; ++i)
            {
                builder.Append(".");
                Visit(namespaceParts[i], builder);
            }
        }

        public void Visit(OdataIdentifier node, StringBuilder builder)
        {
            builder.Append(node.Identifier); //// TODO is there really nothign els eyou need?
        }

        public void Visit(QualifiedEntityTypeName node, StringBuilder builder)
        {
            Visit(node.Namespace, builder);
            builder.Append(".");
            Visit(node.EntityTypeName, builder);
        }

        public void Visit(PrimitiveProperty node, StringBuilder builder)
        {
            if (node is PrimitiveProperty.PrimitiveKeyProperty primitiveKeyProperty)
            {
                Visit(primitiveKeyProperty.Identifier, builder);
            }
            else if (node is PrimitiveProperty.PrimitiveNonKeyProperty primitiveNonKeyProperty)
            {
                Visit(primitiveNonKeyProperty.Identifier, builder);
            }
            else
            {
                throw new Exception("TODO a proper visitor pattern would prevent this branch");
            }
        }

        public void Visit(NavigationProperty node, StringBuilder builder)
        {
            if (node is NavigationProperty.EntityNavigationProperty entityNavigationProperty)
            {
                Visit(entityNavigationProperty.Identifier, builder);
            }
            else if (node is NavigationProperty.EntityCollectionNavigationProperty entityCollectionNavigationProperty)
            {
                Visit(entityCollectionNavigationProperty.Identifier, builder);
            }
            else
            {
                throw new Exception("TODO a proper visitor pattern would prevent this branch");
            }
        }

        public void Visit(QualifiedComplexTypeName node, StringBuilder builder)
        {
            Visit(node.Namespace, builder);
            builder.Append(".");
            Visit(node.EntityTypeName, builder);
        }

        public void Visit(AliasAndValue node, StringBuilder builder)
        {
            throw new Exception("tODO aliasandvalue is not supported yet");
        }

        public void Visit(QualifiedActionName node, StringBuilder builder)
        {
            Visit(node.Namespace, builder);
            builder.Append(".");
            Visit(node.Action, builder);
        }

        public void Visit(QualifiedFunctionName node, StringBuilder builder)
        {
            Visit(node.Namespace, builder);
            builder.Append(".");
            Visit(node.Function, builder);
        }
    }
}
