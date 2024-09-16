namespace Fx.OdataPocRoot.Odata.UriExpressionVisitorImplementations //// TODO better name
{
    using System;

    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Select;
    using System.Linq;
    using System.Text;

    public sealed class SelectToStringVisitor
    {
        private readonly CommonToStringVisitor commonToStringVisitor;

        private readonly FilterToStringVisitor filterToStringVisitor;

        private readonly SearchToStringVisitor searchToStringVisitor;

        private readonly InlineCountToStringVisitor inlineCountToStringVisitor;

        private readonly OrderByToStringVisitor orderByToStringVisitor;

        private readonly SkipToStringVisitor skipToStringVisitor;

        private readonly TopToStringVisitor topToStringVisitor;

        private readonly ComputeToStringVisitor computeToStringVisitor;

        private readonly ExpandToStringVisitor expandToStringVisitor;

        public SelectToStringVisitor()
            : this(
                  CommonToStringVisitor.Default,
                  new FilterToStringVisitor(),
                  new SearchToStringVisitor(),
                  new InlineCountToStringVisitor(),
                  new OrderByToStringVisitor(),
                  new SkipToStringVisitor(),
                  new TopToStringVisitor(),
                  new ComputeToStringVisitor(),
                  new ExpandToStringVisitor())
        {
        }

        public SelectToStringVisitor(CommonToStringVisitor commonToStringVisitor, FilterToStringVisitor filterToStringVisitor, SearchToStringVisitor searchToStringVisitor, InlineCountToStringVisitor inlineCountToStringVisitor, OrderByToStringVisitor orderByToStringVisitor, SkipToStringVisitor skipToStringVisitor, TopToStringVisitor topToStringVisitor, ComputeToStringVisitor computeToStringVisitor, ExpandToStringVisitor expandToStringVisitor)
        {
            this.commonToStringVisitor = commonToStringVisitor;
            this.filterToStringVisitor = filterToStringVisitor;
            this.searchToStringVisitor = searchToStringVisitor;
            this.inlineCountToStringVisitor = inlineCountToStringVisitor;
            this.orderByToStringVisitor = orderByToStringVisitor;
            this.skipToStringVisitor = skipToStringVisitor;
            this.topToStringVisitor = topToStringVisitor;
            this.computeToStringVisitor = computeToStringVisitor;
            this.expandToStringVisitor = expandToStringVisitor;
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

            builder.Append("$select=");
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
                    builder.Append("/");
                    Visit(first.SelectProperty, builder);
                }
                else if (propertyPath is SelectItem.PropertyPath.Second second)
                {
                    Visit(second.SelectProperty, builder);
                }
                else if (propertyPath is SelectItem.PropertyPath.Third third)
                {
                    this.commonToStringVisitor.Visit(third.QualifiedEntityTypeName, builder);
                    builder.Append("/");
                    this.commonToStringVisitor.Visit(third.QualifiedActionName, builder);
                }
                else if (propertyPath is SelectItem.PropertyPath.Fourth fourth)
                {
                    this.commonToStringVisitor.Visit(fourth.QualifiedActionName, builder);
                }
                else if (propertyPath is SelectItem.PropertyPath.Fifth fifth)
                {
                    this.commonToStringVisitor.Visit(fifth.QualifiedEntityTypeName, builder);
                    builder.Append("/");
                    this.commonToStringVisitor.Visit(fifth.QualifiedFunctionName, builder);
                }
                else if (propertyPath is SelectItem.PropertyPath.Sixth sixth)
                {
                    this.commonToStringVisitor.Visit(sixth.QualifiedFunctionName, builder);
                }
                else if (propertyPath is SelectItem.PropertyPath.Seventh seventh)
                {
                    this.commonToStringVisitor.Visit(seventh.QualifiedComplexTypeName, builder);
                    builder.Append("/");
                    Visit(seventh.SelectProperty, builder);
                }
                else if (propertyPath is SelectItem.PropertyPath.Eighth eighth)
                {
                    this.commonToStringVisitor.Visit(eighth.QualifiedComplexTypeName, builder);
                    builder.Append("/");
                    this.commonToStringVisitor.Visit(eighth.QualifiedActionName, builder);
                }
                else if (propertyPath is SelectItem.PropertyPath.Ninth ninth)
                {
                    this.commonToStringVisitor.Visit(ninth.QualifiedComplexTypeName, builder);
                    builder.Append("/");
                    this.commonToStringVisitor.Visit(ninth.QualifiedFunctionName, builder);
                }
                else
                {
                    throw new Exception("TODO a proper visitor pattern would prevent this branch");
                }
            }
            else
            {
                throw new Exception("TODO a proper visitor pattern would prevent this branch");
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
                    var selectOptions = selectOption.SelectOptions.ToList();
                    if (selectOptions.Count == 0)
                    {
                        //// TODO is this actually legal? model it somehow if it's not
                        return;
                    }

                    Action<SelectOption, StringBuilder> visit = (SelectOption node, StringBuilder stringBuilder) =>
                    {
                        if (node is SelectOption.SelectOptionPcNode selectOptionPcNode)
                        {
                            Visit(selectOptionPcNode.SelectOptionPc, stringBuilder);
                        }
                        else if (node is SelectOption.ComputeNode computeNode)
                        {
                            this.computeToStringVisitor.Visit(computeNode.Compute, stringBuilder);
                        }
                        else if (node is SelectOption.SelectNode selectNode)
                        {
                            Visit(selectNode.Select, stringBuilder);
                        }
                        else if (node is SelectOption.ExpandNode expandNode)
                        {
                            this.expandToStringVisitor.Visit(expandNode.Expand, stringBuilder);
                        }
                        else if (node is SelectOption.AliasAndValueNode aliasAndValueNode)
                        {
                            this.commonToStringVisitor.Visit(aliasAndValueNode.AliasAndValue, stringBuilder);
                        }
                        else
                        {
                            throw new Exception("TODO a proper visitor pattern would prevent this branch");
                        }
                    };

                    builder.Append("(");
                    visit(selectOptions[0], builder);
                    for (int i = 0; i < selectOptions.Count; ++i)
                    {
                        builder.Append(";");
                        visit(selectOptions[i], builder);
                    }

                    builder.Append(")");
                }
                else if (node is SelectProperty.FullSelectPath.SelectPropertyNode selectPropertyNode)
                {
                    builder.Append("/");
                    Visit(selectPropertyNode.SelectProperty, builder);
                }
                else
                {
                    throw new Exception("TODO a proper visitor pattern would prevent this branch");
                }
            }
            else
            {
                throw new Exception("TODO a proper visitor pattern would prevent this branch");
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
            else if (node is SelectPath.Third third)
            {
                this.commonToStringVisitor.Visit(third.ComplexCollectionProperty, builder);
            }
            else if (node is SelectPath.Fourth fourth)
            {
                this.commonToStringVisitor.Visit(fourth.ComplexCollectionProperty, builder);
                builder.Append("/");
                this.commonToStringVisitor.Visit(fourth.QualifiedComplexTypeName, builder);
            }
            else
            {
                throw new Exception("TODO a proper visitor pattern would prevent this branch");
            }
        }
    }
}
