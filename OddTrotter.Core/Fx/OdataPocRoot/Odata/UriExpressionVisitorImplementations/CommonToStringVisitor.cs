namespace Fx.OdataPocRoot.Odata.UriExpressionVisitorImplementations
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;

    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;

    public sealed class CommonToStringVisitor
    {
        private CommonToStringVisitor()
        {
            //// TODO configure how much whitespace to use for BWS?
        }

        public static CommonToStringVisitor Default { get; } = new CommonToStringVisitor();

        public void Visit(Fx.OdataPocRoot.Odata.UriExpressionNodes.Common.Enum node, StringBuilder builder)
        {
            if (node is Fx.OdataPocRoot.Odata.UriExpressionNodes.Common.Enum.Qualified qualified)
            {
                Visit(qualified.QualifiedEnumTypeName, builder);
                builder.Append("'");
                Visit(qualified.EnumValue, builder);
                builder.Append("'");
            }
            else if (node is Fx.OdataPocRoot.Odata.UriExpressionNodes.Common.Enum.Unqualified unqualified)
            {
                builder.Append("'");
                Visit(unqualified.EnumValue, builder);
                builder.Append("'");
            }
            else
            {
                throw new System.Exception("TODO a proper visitor would prevent this branch");
            }
        }

        public void Visit(EnumValue node, StringBuilder builder)
        {
            using (var enumerator = node.SingleEnumValues.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw new Exception("TODO can't reach...");
                }

                Visit(enumerator.Current, builder);
                while (enumerator.MoveNext())
                {
                    builder.Append(",");
                    Visit(enumerator.Current, builder);
                }
            }
        }

        public void Visit(SingleEnumValue node, StringBuilder builder)
        {
            if (node is SingleEnumValue.EnumerationMemberNode enumerationMemberNode)
            {
                Visit(enumerationMemberNode.EnumerationMember, builder);
            }
            else if (node is SingleEnumValue.EnumMemberValueNode enumMemberValueNode)
            {
                builder.Append(enumMemberValueNode.EnumMemberValue);
            }
            else
            {
                throw new System.Exception("TODO a proper visitor would prevent this branch");
            }
        }

        public void Visit(IsofExpression node, StringBuilder builder)
        {
            builder.Append("isof");
            builder.Append("(");
            Visit(node.CommonExpression, builder);
            builder.Append(",");
            Visit(node.QualifiedTypeName, builder);
            builder.Append(")");
        }

        public void Visit(QualifiedTypeName node, StringBuilder builder)
        {
            if (node is QualifiedTypeName.SingleValue singleValue)
            {
                Visit(singleValue.SingleQualifiedTypeName, builder);
            }
            else if (node is QualifiedTypeName.MultiValue multiValue)
            {
                builder.Append("Collection");
                builder.Append("(");
                Visit(multiValue.SingleQualifiedTypeName, builder);
                builder.Append(")");
            }
            else
            {
                throw new System.Exception("TODO a proper visitor would prevent this branch");
            }
        }

        public void Visit(SingleQualifiedTypeName node, StringBuilder builder)
        {
            if (node is SingleQualifiedTypeName.QualifiedEntityType qualifiedEntityType)
            {
                Visit(qualifiedEntityType.QualifiedEntityTypeName, builder);
            }
            else if (node is SingleQualifiedTypeName.QualifiedComplexType qualifiedComplexType)
            {
                Visit(qualifiedComplexType.QualifiedComplexTypeName, builder);
            }
            else if (node is SingleQualifiedTypeName.QualifiedTypeDefinition qualifiedTypeDefinition)
            {
                Visit(qualifiedTypeDefinition.QualifiedTypeDefinitionName, builder);
            }
            else if (node is SingleQualifiedTypeName.QualifiedEnumType qualifiedEnumType)
            {
                Visit(qualifiedEnumType.QualifiedEnumTypeName, builder);
            }
            else if (node is SingleQualifiedTypeName.PrimitiveType primitiveType)
            {
                Visit(primitiveType.PrimitiveTypeName, builder);
            }
        }

        public void Visit(PrimitiveTypeName node, StringBuilder builder)
        {
            builder.Append("Edm.");
            if (node is PrimitiveTypeName.Binary binary)
            {
                builder.Append("Binary");
            }
            else if (node is PrimitiveTypeName.Boolean boolean)
            {
                builder.Append("Boolean");
            }
            else if (node is PrimitiveTypeName.Byte @byte)
            {
                builder.Append("Byte");
            }
            else if (node is PrimitiveTypeName.Date date)
            {
                builder.Append("Date");
            }
            else if (node is PrimitiveTypeName.DateTimeOffset dateTimeOffset)
            {
                builder.Append("DateTimeOffset");
            }
            else if (node is PrimitiveTypeName.Decimal @decimal)
            {
                builder.Append("Decimal");
            }
            else if (node is PrimitiveTypeName.Double @double)
            {
                builder.Append("Double");
            }
            else if (node is PrimitiveTypeName.Duration duration)
            {
                builder.Append("Duration");
            }
            else if (node is PrimitiveTypeName.Guid guid)
            {
                builder.Append("Guid");
            }
            else if (node is PrimitiveTypeName.Int16 int16)
            {
                builder.Append("Int16");
            }
            else if (node is PrimitiveTypeName.Int32 int32)
            {
                builder.Append("Int32");
            }
            else if (node is PrimitiveTypeName.Int64 int64)
            {
                builder.Append("Int64");
            }
            else if (node is PrimitiveTypeName.Sbyte @sbyte)
            {
                builder.Append("SByte");
            }
            else if (node is PrimitiveTypeName.Single single)
            {
                builder.Append("Single");
            }
            else if (node is PrimitiveTypeName.Stream stream)
            {
                builder.Append("Stream");
            }
            else if (node is PrimitiveTypeName.String @string)
            {
                builder.Append("String");
            }
            else if (node is PrimitiveTypeName.TimeOfDay timeOfDay)
            {
                builder.Append("TimeOfDay");
            }
            else
            {
                throw new System.Exception("TODO implement this when you've done the full ABNF");
            }
        }

        public void Visit(QualifiedEnumTypeName node, StringBuilder builder)
        {
            Visit(node.Namespace, builder);
            builder.Append(".");
            Visit(node.EnumerationTypeName, builder);
        }

        public void Visit(QualifiedTypeDefinitionName node, StringBuilder builder)
        {
            Visit(node.Namespace, builder);
            builder.Append(".");
            Visit(node.TypeDefinitionName, builder);
        }

        public void Visit(EndsWithMethodCallExpression node, StringBuilder builder)
        {
            builder.Append("endswith");
            builder.Append("(");
            Visit(node.Left, builder);
            builder.Append(",");
            Visit(node.Right, builder);
            builder.Append(")");
        }

        public void Visit(StartsWithMethodCallExpression node, StringBuilder builder)
        {
            builder.Append("startswith");
            builder.Append("(");
            Visit(node.Left, builder);
            builder.Append(",");
            Visit(node.Right, builder);
            builder.Append(")");
        }

        public void Visit(ContainsMethodCallExpression node, StringBuilder builder)
        {
            builder.Append("contains");
            builder.Append("(");
            Visit(node.Left, builder);
            builder.Append(",");
            Visit(node.Right, builder);
            builder.Append(")");
        }

        public void Visit(IntersectsMethodCallExpression node, StringBuilder builder)
        {
            builder.Append("geo.intersects");
            builder.Append("(");
            Visit(node.Left, builder);
            builder.Append(",");
            Visit(node.Right, builder);
            builder.Append(")");
        }

        public void Visit(HasSubsetMethodCallExpression node, StringBuilder builder)
        {
            builder.Append("hassubset");
            builder.Append("(");
            Visit(node.Left, builder);
            builder.Append(",");
            Visit(node.Right, builder);
            builder.Append(")");
        }

        public void Visit(HasSubsequenceMethodCallExpression node, StringBuilder builder)
        {
            builder.Append("hassubsequence");
            builder.Append("(");
            Visit(node.Left, builder);
            builder.Append(",");
            Visit(node.Right, builder);
            builder.Append(")");
        }

        public void Visit(CommonExpression node, StringBuilder builder)
        {
            //// TODO implement this when you've done the full ABNF
            if (node is CommonExpression.Todo todo)
            {
                Visit(todo.Identifier, builder);
                builder.Append("/");
                Visit(todo.CommonExpression, builder);
            }
            else if (node is CommonExpression.TodoTerminal todoTerminal)
            {
                Visit(todoTerminal.Identifier, builder);
            }
            else
            {
                throw new System.Exception("TODO a proper visitor would prevent this branch");
            }
        }

        public void Visit(ImplicitVariableExpression node, StringBuilder builder)
        {
            if (node is ImplicitVariableExpression.It it)
            {
                builder.Append("$it");
            }
            else if (node is ImplicitVariableExpression.This @this)
            {
                builder.Append("$this");
            }
            else
            {
                throw new System.Exception("TODO a proper visitor would prevent this branch");
            }
        }

        public void Visit(FunctionExpression node, StringBuilder builder)
        {
            throw new System.Exception("TODO implement this when you've done the full ABNF");
        }

        public void Visit(KeyPredicate node, StringBuilder builder)
        {
            if (node is KeyPredicate.SimpleKey simpleKey)
            {
                Visit(simpleKey, builder);
            }
            else
            {
                throw new System.Exception("TODO implement this when you've done the full ABNF");
            }
        }

        public void Visit(KeyPredicate.SimpleKey node, StringBuilder builder)
        {
            builder.Append("(");
            if (node is KeyPredicate.SimpleKey.ParameterAliasNode parameterAliasNode)
            {
                Visit(parameterAliasNode, builder);
            }
            else if (node is KeyPredicate.SimpleKey.KeyPropertyValueNode keyPropertyValueNode)
            {
                Visit(keyPropertyValueNode.PrimitiveLiteral, builder);
            }
            else
            {
                throw new System.Exception("TODO a proper visitor would prevent this branch");
            }

            builder.Append(")");
        }

        public void Visit(KeyPredicate.SimpleKey.KeyPropertyValueNode node, StringBuilder builder)
        {
            Visit(node.PrimitiveLiteral, builder);
        }

        public void Visit(PrimitiveLiteral node, StringBuilder builder)
        {
            if (node is PrimitiveLiteral.NullValue nullValue)
            {
                builder.Append("null");
            }
            else if (node is PrimitiveLiteral.BooleanValueNode booleanValueNode)
            {
                Visit(booleanValueNode.BooleanValue, builder);
            }
            else if (node is PrimitiveLiteral.GuidValue guidValueNode)
            {
                builder.Append(guidValueNode.Guid.ToString());
            }
            else if (node is PrimitiveLiteral.DateValue dateTime)
            {
                //// TODO make sure you did this right
                builder.Append(dateTime.Date.Year);
                builder.Append("-");
                builder.Append(dateTime.Date.Month);
                builder.Append("-");
                builder.Append(dateTime.Date.Day);
            }
            else if (node is PrimitiveLiteral.DateTimeOffsetValue dateTimeOffsetValue)
            {
                //// TODO make sure you did this right
                //// TODO make the Z vs hour-offset configurable
                //// TODO make it configurable how much of the optional stuff is included...
                builder.Append(dateTimeOffsetValue.DateTimeOffset.Year);
                builder.Append("-");
                builder.Append(dateTimeOffsetValue.DateTimeOffset.Month);
                builder.Append("-");
                builder.Append(dateTimeOffsetValue.DateTimeOffset.Day);
                builder.Append("T");
                builder.Append(dateTimeOffsetValue.DateTimeOffset.Hour);
                builder.Append(":");
                builder.Append(dateTimeOffsetValue.DateTimeOffset.Minute);
                builder.Append(":");
                builder.Append(dateTimeOffsetValue.DateTimeOffset.Second);
                builder.Append(".");
                builder.Append(dateTimeOffsetValue.DateTimeOffset.Nanosecond);
                builder.Append(dateTimeOffsetValue.DateTimeOffset.Offset.Ticks < 0 ? "-" : "+");
                builder.Append(dateTimeOffsetValue.DateTimeOffset.Offset.Hours);
                builder.Append(":");
                builder.Append(dateTimeOffsetValue.DateTimeOffset.Offset.Minutes);
            }
            else if (node is PrimitiveLiteral.TimeOfDayValue timeOfDayValue)
            {
                builder.Append(timeOfDayValue.TimeOfDay.Hour);
                builder.Append(":");
                builder.Append(timeOfDayValue.TimeOfDay.Minute);
                builder.Append(":");
                builder.Append(timeOfDayValue.TimeOfDay.Second);
                builder.Append(".");
                builder.Append(timeOfDayValue.TimeOfDay.Nanosecond);
            }
            else if (node is PrimitiveLiteral.DecimalValue decimalValue)
            {
                //// TODO can you trust this?
                builder.Append(decimalValue.Decimal);
            }
            else if (node is PrimitiveLiteral.DoubleValue doubleValue)
            {
                builder.Append(doubleValue.Double);
            }
            else if (node is PrimitiveLiteral.SingleValue singleValue)
            {
                builder.Append(singleValue.Single);
            }
            else if (node is PrimitiveLiteral.SbyteValue sbyteValue)
            {
                builder.Append(sbyteValue.Sbyte);
            }
            else if (node is PrimitiveLiteral.ByteValue byteValue)
            {
                builder.Append(byteValue.Byte);
            }
            else if (node is PrimitiveLiteral.Int16Value int16Value)
            {
                builder.Append(int16Value.Int16);
            }
            else if (node is PrimitiveLiteral.Int32Value int32Value)
            {
                builder.Append(int32Value.Int32);
            }
            else if (node is PrimitiveLiteral.Int64Value int64Value)
            {
                builder.Append(int64Value.Int64);
            }
            else if (node is PrimitiveLiteral.StringValue stringValue)
            {
                builder.Append("'");
                builder.Append(stringValue.String.Replace("'", "''"));
                builder.Append("'");
            }
            else if (node is PrimitiveLiteral.DurationValue durationValue)
            {
                //// TODO make it configuration if you include the "duration" constant string
                throw new Exception("TODO actually implement this correctly");
            }
            else if (node is PrimitiveLiteral.EnumValue enumValue)
            {
                throw new System.Exception("TODO implement this when you've done the full ABNF");
            }
            else if (node is PrimitiveLiteral.BinaryValue binaryValue)
            {
                builder.Append("binary");
                builder.Append("'");
                //// TODO you're not base64*URL* encoding, just base64 encoding...
                builder.Append(Convert.ToBase64String(binaryValue.Binary));
                builder.Append("'");
            }
            else
            {
                throw new System.Exception("TODO implement this when you've done the full ABNF");
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
                throw new System.Exception("TODO a proper visitor would prevent this branch");
            }
        }

        public void Visit(KeyPredicate.SimpleKey.ParameterAliasNode node, StringBuilder builder)
        {
            builder.Append("@"); //// TODO should parameterAlias be its own type, not a nested type?
            Visit(node.ParameterAlias, builder);
        }

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
