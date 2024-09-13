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

        private readonly SearchToStringVisitor searchToStringVisitor;

        private readonly InlineCountToStringVisitor inlineCountToStringVisitor;

        private readonly OrderByToStringVisitor orderByToStringVisitor;

        private readonly SkipToStringVisitor skipToStringVisitor;

        private readonly TopToStringVisitor topToStringVisitor;

        public SelectToStringVisitor(FilterToStringVisitor filterToStringVisitor, SearchToStringVisitor searchToStringVisitor, InlineCountToStringVisitor inlineCountToStringVisitor, OrderByToStringVisitor orderByToStringVisitor, SkipToStringVisitor skipToStringVisitor, TopToStringVisitor topToStringVisitor)
        {
            this.filterToStringVisitor = filterToStringVisitor;
            this.searchToStringVisitor = searchToStringVisitor;
            this.inlineCountToStringVisitor = inlineCountToStringVisitor;
            this.orderByToStringVisitor = orderByToStringVisitor;
            this.skipToStringVisitor = skipToStringVisitor;
            this.topToStringVisitor = topToStringVisitor;
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
                Visit(nestedOptions[0], builder);
                for (int i = 1; i < nestedOptions.Count; ++i)
                {
                    builder.Append(";");
                    Visit(nestedOptions[i], builder);
                }

                builder.Append(")");
            }
            else if (node is SelectProperty.NavigationProperty navigationProperty)
            {
                this.commonToStringVisitor.Visit(navigationProperty.Property, builder);
            }
            else if (node is SelectProperty.FullSelectPath fullSelectPath)
            {
                Visit(fullSelectPath.SelectPath, builder);
                if (node is SelectProperty.FullSelectPath.SelectOption selectOption)
                {
                }
            }
        }

        public void Visit(SelectOptionPc node, StringBuilder builder)
        {
            if (node is SelectOptionPc.FilterNode filterNode)
            {
                this.filterToStringVisitor.Visit(filterNode.Filter, builder);
            }
            else if (node is SelectOptionPc.SearchNode searchNode)
            {
                this.searchToStringVisitor.Visit(searchNode.Search, builder);
            }
            else if (node is SelectOptionPc.InlineCountNode inlineCountNode)
            {
                this.inlineCountToStringVisitor.Visit(inlineCountNode.InlineCount, builder);
            }
            else if (node is SelectOptionPc.OrderByNode orderByNode)
            {
                this.orderByToStringVisitor.Visit(orderByNode.OrderBy, builder);
            }
            else if (node is SelectOptionPc.SkipNode skipNode)
            {
                this.skipToStringVisitor.Visit(skipNode.Skip, builder);
            }
            else if (node is SelectOptionPc.TopNode topNode)
            {
                this.topToStringVisitor.Visit(topNode.Top, builder);
            }
            else
            {
                throw new Exception("TODO a proper visitor pattern would prevent this branch");
            }
        }

        public void Visit(SelectPath node, StringBuilder builder)
        {
            if (node is SelectPath.First first)
            {
                this.commonToStringVisitor.Visit(first.ComplexProperty, builder);
            }
            else if (node is SelectPath.Second second)
            {
                this.commonToStringVisitor.Visit(second.ComplexProperty, builder);
                builder.Append("/");
                this.commonToStringVisitor.Visit(second.QualifiedComplexTypeName, builder);
            }
            else if ()
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
        }
    }
}
