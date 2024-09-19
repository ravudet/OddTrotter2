namespace Fx.OdataPocRoot.Odata.UriExpressionVisitorImplementations
{
    using System.Text;

    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter;

    public sealed class FilterToStringVisitor
    {
        private readonly CommonToStringVisitor commonToStringVisitor;

        public FilterToStringVisitor(CommonToStringVisitor commonToStringVisitor)
        {
            this.commonToStringVisitor = commonToStringVisitor;
        }

        public void Visit(Filter node, StringBuilder builder)
        {
            builder.Append("$filter=");
            Visit(node.BoolCommonExpression, builder);
        }

        public void Visit(BoolCommonExpression node, StringBuilder builder)
        {
            if (node is BoolCommonExpression.First first)
            {
                this.commonToStringVisitor.Visit(first.BooleanValue, builder);
            }
            else if (node is BoolCommonExpression.Second second)
            {
                Visit(second.BoolRootExpression, builder);
            }
            else if (node is BoolCommonExpression.Third third)
            {
                Visit(third.BoolFirstMemberExpression, builder);
            }
            else if (node is BoolCommonExpression.Fourth fourth)
            {
                Visit(fourth.BoolFunctionExpression, builder);
            }
            else if (node is BoolCommonExpression.Fifth fifth)
            {
                Visit(fifth.BoolMethodCallExpression, builder);
            }
            else if (node is BoolCommonExpression.Sixth sixth)
            {
                Visit(sixth.BoolParenExpression, builder);
            }
            else if (node is BoolCommonExpression.Seventh seventh)
            {
                throw new System.Exception("TODO implement this when you've done the full ABNF");
            }
            else if (node is BoolCommonExpression.Eighth eighth)
            {
                this.commonToStringVisitor.Visit(eighth.IsofExpression, builder);
            }
            else if (node is BoolCommonExpression.Ninth ninth)
            {
                Visit(ninth.NotExpression, builder);
            }
            else if (node is BoolCommonExpression.Tenth tenth)
            {
                this.commonToStringVisitor.Visit(tenth.PrimitiveLiteral, builder);
                Visit(tenth.EqualsExpression, builder);
            }
            else if (node is BoolCommonExpression.Eleventh eleventh)
            {
                this.commonToStringVisitor.Visit(eleventh.PrimitiveLiteral, builder);
                Visit(eleventh.NotEqualsExpression, builder);
            }
            else if (node is BoolCommonExpression.Twelfth twelfth)
            {
                this.commonToStringVisitor.Visit(twelfth.PrimitiveLiteral, builder);
                Visit(twelfth.LessThanExpression, builder);
            }
            else if (node is BoolCommonExpression.Thirteenth thirteenth)
            {
                this.commonToStringVisitor.Visit(thirteenth.PrimitiveLiteral, builder);
                Visit(thirteenth.LessThanOrEqualToExpression, builder);
            }
            else if (node is BoolCommonExpression.Fourteenth fourteenth)
            {
                this.commonToStringVisitor.Visit(fourteenth.PrimitiveLiteral, builder);
                Visit(fourteenth.GreaterThanExpression, builder);
            }
            else if (node is BoolCommonExpression.Fifteenth fifteenth)
            {
                this.commonToStringVisitor.Visit(fifteenth.PrimitiveLiteral, builder);
                Visit(fifteenth.GreatherThanOrEqualToExpression, builder);
            }
            else if (node is BoolCommonExpression.Sixteenth sixteenth)
            {
                this.commonToStringVisitor.Visit(sixteenth.PrimitiveLiteral, builder);
                Visit(sixteenth.HasExpression, builder);
            }
            else if (node is BoolCommonExpression.Seventeenth seventeenth)
            {
                this.commonToStringVisitor.Visit(seventeenth.PrimitiveLiteral, builder);
                Visit(seventeenth.InExpression, builder);
            }
            else if (node is BoolCommonExpression.Eighteenth eighteenth)
            {
                Visit(eighteenth.BoolCommonExpression, builder);
                Visit(eighteenth.AndExpression, builder);
            }
            else if (node is BoolCommonExpression.Nineteenth nineteenth)
            {
                Visit(nineteenth.BoolCommonExpression, builder);
                Visit(nineteenth.OrExpression, builder);
            }
            else
            {
                throw new System.Exception("TODO a proper visitor would prevent this branch");
            }
        }

        public void Visit(OrExpression node, StringBuilder builder)
        {
            builder.Append(" ");
            builder.Append("or");
            builder.Append(" ");
            Visit(node.Right, builder);
        }

        public void Visit(AndExpression node, StringBuilder builder)
        {
            builder.Append(" ");
            builder.Append("and");
            builder.Append(" ");
            Visit(node.Right, builder);
        }

        public void Visit(InExpression node, StringBuilder builder)
        {
            builder.Append(" ");
            builder.Append("in");
            builder.Append(" ");
            this.commonToStringVisitor.Visit(node.Right, builder);
        }

        public void Visit(HasExpression node, StringBuilder builder)
        {
            builder.Append(" ");
            builder.Append("eq");
            builder.Append(" ");
            this.commonToStringVisitor.Visit(node.Enum, builder);
        }

        public void Visit(GreatherThanOrEqualToExpression node, StringBuilder builder)
        {
            builder.Append(" ");
            builder.Append("ge");
            builder.Append(" ");
            this.commonToStringVisitor.Visit(node.Right, builder);
        }

        public void Visit(GreaterThanExpression node, StringBuilder builder)
        {
            builder.Append(" ");
            builder.Append("gt");
            builder.Append(" ");
            this.commonToStringVisitor.Visit(node.Right, builder);
        }

        public void Visit(LessThanOrEqualToExpression node, StringBuilder builder)
        {
            builder.Append(" ");
            builder.Append("le");
            builder.Append(" ");
            this.commonToStringVisitor.Visit(node.Right, builder);
        }

        public void Visit(LessThanExpression node, StringBuilder builder)
        {
            builder.Append(" ");
            builder.Append("lt");
            builder.Append(" ");
            this.commonToStringVisitor.Visit(node.Right, builder);
        }

        public void Visit(NotEqualsExpression node, StringBuilder builder)
        {
            builder.Append(" ");
            builder.Append("ne");
            builder.Append(" ");
            this.commonToStringVisitor.Visit(node.Right, builder);
        }

        public void Visit(EqualsExpression node, StringBuilder builder)
        {
            builder.Append(" ");
            builder.Append("eq");
            builder.Append(" ");
            this.commonToStringVisitor.Visit(node.Right, builder);
        }

        public void Visit(NotExpression node, StringBuilder builder)
        {
            builder.Append("not");
            builder.Append(" ");
            Visit(node.BoolCommonExpression, builder);
        }

        public void Visit(BoolParenExpression node, StringBuilder builder)
        {
            builder.Append("(");
            Visit(node.BoolCommonExpression, builder);
            builder.Append(")");
        }

        private void Visit(BoolMethodCallExpression node, StringBuilder builder)
        {
            if (node is BoolMethodCallExpression.EndsWith endsWith)
            {
                this.commonToStringVisitor.Visit(endsWith.EndsWithMethodCallExpression, builder);
            }
            else if (node is BoolMethodCallExpression.StartsWith startsWith)
            {
                this.commonToStringVisitor.Visit(startsWith.StartsWithMethodCallExpression, builder);
            }
            else if (node is BoolMethodCallExpression.Contains contains)
            {
                this.commonToStringVisitor.Visit(contains.ContainsMethodCallExpression, builder);
            }
            else if (node is BoolMethodCallExpression.Intersects intersects)
            {
                this.commonToStringVisitor.Visit(intersects.IntersectsMethodCallExpression, builder);
            }
            else if (node is BoolMethodCallExpression.HasSubset hasSubset)
            {
                this.commonToStringVisitor.Visit(hasSubset.HasSubsetMethodCallExpression, builder);
            }
            else if (node is BoolMethodCallExpression.HasSubsequence hasSubsequence)
            {
                this.commonToStringVisitor.Visit(hasSubsequence.HasSubsequenceMethodCallExpression, builder);
            }
            else
            {
                throw new System.Exception("TODO a proper visitor would prevent this branch");
            }
        }

        private void Visit(BoolFunctionExpression node, StringBuilder builder)
        {
            throw new System.Exception("TODO implement this when you've done the full ABNF");
        }

        private void Visit(BoolFirstMemberExpression node, StringBuilder builder)
        {
            if (node is BoolFirstMemberExpression.BoolMemberExpressionNode boolMemberExpression)
            {
                Visit(boolMemberExpression.BoolMemberExpression, builder);
            }
            else if (node is BoolFirstMemberExpression.InscopeVariable inscopeVariable)
            {
                Visit(inscopeVariable.BoolInscopeVariableExpression, builder);
                if (inscopeVariable is BoolFirstMemberExpression.InscopeVariable.Variable variable)
                {
                }
                else if (inscopeVariable is BoolFirstMemberExpression.InscopeVariable.Member member)
                {
                    builder.Append("/");
                    Visit(member.BoolMemberExpression, builder);
                }
                else
                {
                    throw new System.Exception("TODO a proper visitor would prevent this branch");
                }
            }
            else
            {
                throw new System.Exception("TODO a proper visitor would prevent this branch");
            }
        }

        private void Visit(BoolInscopeVariableExpression node, StringBuilder builder)
        {
            if (node is BoolInscopeVariableExpression.ImplicitVariable implicitVariable)
            {
                this.commonToStringVisitor.Visit(implicitVariable.ImplicitVariableExpression, builder);
            }
            else if (node is BoolInscopeVariableExpression.ParameterAliasNode parameterAliasNode)
            {
                this.commonToStringVisitor.Visit(parameterAliasNode.ParameterAlias, builder);
            }
            else
            {
                throw new System.Exception("TODO a proper visitor would prevent this branch");
            }
        }

        private void Visit(BoolMemberExpression node, StringBuilder builder)
        {
            if (node is BoolMemberExpression.Qualified qualified)
            {
                this.commonToStringVisitor.Visit(qualified.QualifiedEntityTypeName, builder);
                builder.Append("/");
                if (qualified is BoolMemberExpression.Qualified.PropertyPath propertyPath)
                {
                    Visit(propertyPath.BoolPropertyPathExpression, builder);
                }
                else if (qualified is BoolMemberExpression.Qualified.BoundFunction boundFunction)
                {
                    this.commonToStringVisitor.Visit(boundFunction.BoundFunctionExpression, builder);
                }
                else if (qualified is BoolMemberExpression.Qualified.BoolAnnotation annotation)
                {
                    Visit(annotation.BoolAnnotationExpression, builder);
                }
                else
                {
                    throw new System.Exception("TODO a proper visitor would prevent this branch");
                }
            }
            else if (node is BoolMemberExpression.Unqualified unqualified)
            {
                if (unqualified is BoolMemberExpression.Unqualified.PropertyPath propertyPath)
                {
                    Visit(propertyPath.BoolPropertyPathExpression, builder);
                }
                else if (unqualified is BoolMemberExpression.Unqualified.BoundFunction boundFunction)
                {
                    this.commonToStringVisitor.Visit(boundFunction.BoundFunctionExpression, builder);
                }
                else if (unqualified is BoolMemberExpression.Unqualified.BoolAnnotation annotation)
                {
                    Visit(annotation.BoolAnnotationExpression, builder);
                }
                else
                {
                    throw new System.Exception("TODO a proper visitor would prevent this branch");
                }
            }
            else
            {
                throw new System.Exception("TODO a proper visitor would prevent this branch");
            }
        }

        private void Visit(BoolPropertyPathExpression node, StringBuilder builder)
        {
            if (node is BoolPropertyPathExpression.EntityCollection entityCollection)
            {
                this.commonToStringVisitor.Visit(entityCollection.EntityCollectionNavigationProperty, builder);
                Visit(entityCollection.BoolCollectionNavigationExpression, builder);
            }
            else if (node is BoolPropertyPathExpression.Entity entity)
            {
                this.commonToStringVisitor.Visit(entity.EntityNavigationProperty, builder);
                Visit(entity.BoolSingleNavigationExpression, builder);
            }
            else if (node is BoolPropertyPathExpression.ComplexCollection complexCollection)
            {
                this.commonToStringVisitor.Visit(complexCollection.ComplexCollectionProperty, builder);
                Visit(complexCollection.BoolCollectionPathExpression, builder);
            }
            else if (node is BoolPropertyPathExpression.PrimitiveCollection primitiveCollection)
            {
                this.commonToStringVisitor.Visit(primitiveCollection.PrimitiveCollectionProperty, builder);
                Visit(primitiveCollection.BoolCollectionPathExpression, builder);
            }
            else if (node is BoolPropertyPathExpression.PrimitveNode primitiveNode)
            {
                if (primitiveNode is BoolPropertyPathExpression.PrimitveNode.Primitive primitive)
                {
                    this.commonToStringVisitor.Visit(primitive.PrimitiveProperty, builder);
                }
                else if (primitiveNode is BoolPropertyPathExpression.PrimitveNode.PrimitiveExpression primitveExpression)
                {
                    this.commonToStringVisitor.Visit(primitveExpression.PrimitiveProperty, builder);
                    Visit(primitveExpression.BoolPrimitivePathExpression, builder);
                }
                else
                {
                    throw new System.Exception("TODO a proper visitor would prevent this branch");
                }
            }
            else if (node is BoolPropertyPathExpression.Stream stream)
            {
                this.commonToStringVisitor.Visit(stream.StreamProperty, builder);
                Visit(stream.BoolPrimitivePathExpression, builder);
            }
            else
            {
                throw new System.Exception("TODO a proper visitor would prevent this branch");
            }
        }

        private void Visit(BoolPrimitivePathExpression node, StringBuilder builder)
        {
            builder.Append("/");
            if (node is BoolPrimitivePathExpression.Annotation annotation)
            {
                Visit(annotation.BoolAnnotationExpression, builder);
            }
            else if (node is BoolPrimitivePathExpression.BoundFunction boundFunction)
            {
                this.commonToStringVisitor.Visit(boundFunction.BoundFunctionExpression, builder);
            }
            else
            {
                throw new System.Exception("TODO a proper visitor would prevent this branch");
            }
        }

        private void Visit(BoolAnnotationExpression node, StringBuilder builder)
        {
            throw new System.Exception("TODO implement this when you've done the full ABNF");
        }

        private void Visit(BoolCollectionNavigationExpression node, StringBuilder builder)
        {
            if (node is BoolCollectionNavigationExpression.First first)
            {
                builder.Append("/");
                this.commonToStringVisitor.Visit(first.QualifiedEntityTypeName, builder);
                this.commonToStringVisitor.Visit(first.KeyPredicate, builder);
                Visit(first.BoolSingleNavigationExpression, builder);
            }
            else if (node is BoolCollectionNavigationExpression.Second second)
            {
                builder.Append("/");
                this.commonToStringVisitor.Visit(second.QualifiedEntityTypeName, builder);
                Visit(second.BoolCollectionPathExpression, builder);
            }
            else if (node is BoolCollectionNavigationExpression.Third third)
            {
                this.commonToStringVisitor.Visit(third.KeyPredicate, builder);
                Visit(third.BoolSingleNavigationExpression, builder);
            }
            else if (node is BoolCollectionNavigationExpression.Fourth fourth)
            {
                Visit(fourth.BoolCollectionPathExpression, builder);
            }
            else
            {
                throw new System.Exception("TODO a proper visitor would prevent this branch");
            }
        }

        private void Visit(BoolCollectionPathExpression node, StringBuilder builder)
        {
            throw new System.Exception("TODO implement this when you've done the full ABNF");
        }

        private void Visit(BoolSingleNavigationExpression node, StringBuilder builder)
        {
            builder.Append("/");
            Visit(node.BoolMemberExpression, builder);
        }

        private void Visit(BoolRootExpression node, StringBuilder builder)
        {
            throw new System.Exception("TODO implement this when you've done the full ABNF");
        }
    }
}
