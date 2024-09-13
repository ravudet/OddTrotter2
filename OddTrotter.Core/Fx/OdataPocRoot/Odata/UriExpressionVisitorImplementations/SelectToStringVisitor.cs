namespace Fx.OdataPocRoot.Odata.UriExpressionVisitorImplementations //// TODO better name
{
    using System;

    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Select;
    using Fx.OdataPocRoot.Odata.UriExpressionVisitors;
    using System.Linq;
    using System.Text;
    using System.Threading;

    public sealed class SelectToStringVisitor
    {
        private readonly CommonToStringVisitor commonToStringVisitor = CommonToStringVisitor.Default; //// TODO factor this to a common place and use constructor injection

        private readonly FilterToStringVisitor filterToStringVisitor;

        public SelectToStringVisitor(FilterToStringVisitor filterToStringVisitor)
        {
            this.filterToStringVisitor = filterToStringVisitor;
        }

        public void Visit(Select node, StringBuilder builder)
        {
            //// TODO should the node have a readonlylist instead of an ienumerable for selectitems?
            var selectItems = node.SelectItems.ToList();
            if (selectItems.Count == 0)
            {
                //// TODO is this actually legal? model it somehow if it's not
                return;
            }

            Visit(selectItems[0], builder);
            for (int i = 1; i < selectItems.Count; ++i)
            {
                builder.Append(",");
                Visit(selectItems[i], builder);
            }
        }

        public void Visit(SelectItem node, StringBuilder builder)
        {
            //// TODO have individual visit methods?
            if (node is SelectItem.Star star)
            {
                builder.Append("*");
            }
            else if (node is SelectItem.AllOperationsInSchema allOperationsInSchema)
            {
                this.commonToStringVisitor.Visit(allOperationsInSchema.SchemaNamespace, builder);
                builder.Append(".*");
            }
            else if (node is SelectItem.PropertyPath propertyPath)
            {
                if (propertyPath is SelectItem.PropertyPath.First first)
                {
                    this.commonToStringVisitor.Visit(first.QualifiedEntityTypeName, builder);
                }
            }
        }

        public void Visit(SelectProperty node, StringBuilder builder)
        {
            if (node is SelectProperty.PrimitiveProperty primitiveProperty)
            {
                this.commonToStringVisitor.Visit(primitiveProperty.Property, builder);
            }
            else if (node is SelectProperty.PrimitiveCollectionProperty primitiveCollectionProperty)
            {
                var nestedOptions = primitiveCollectionProperty.NestedOptions.ToList();
                if (nestedOptions.Count == 0)
                {
                    //// TODO is this actually legal? model it somehow if it's not
                    return;
                }

                this.commonToStringVisitor.Visit(primitiveCollectionProperty.Property, builder);
                builder.Append("(");
                
                builder.Append(")");
            }
        }

        public void Visit(SelectOptionPc node, StringBuilder builder)
        {
            if (node is SelectOptionPc.FilterNode filterNode)
            {
                this.filterToStringVisitor.Visit(filterNode.Filter, builder);
            }
        }

        private sealed class CommonToStringVisitor
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
        }
    }
}
