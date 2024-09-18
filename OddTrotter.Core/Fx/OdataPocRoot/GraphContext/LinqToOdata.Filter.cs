namespace Fx.OdataPocRoot.GraphContext
{
    using Fx.OdataPocRoot.Graph;
    using Fx.OdataPocRoot.Odata;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Select;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public static partial class LinqToOdata
    {
        public static Filter Filter<TType>(Expression<Func<TType, bool>> predicate)
        {
            return FilterUtilities.TraverseFilter(predicate.Body);

            ////return new Filter(new BoolCommonExpression.First(new BooleanValue.True()));
        }

        private static class FilterUtilities
        {
            public static Filter TraverseFilter(Expression expression)
            {
                //// TODO do this in its completeness
                if (expression is ConstantExpression constantExpression)
                {
                    var traversed = TraverseConstantExpression(constantExpression);
                    if (traversed.BooleanValue == null)
                    {
                        throw new Exception("tODO");
                    }

                    return new Filter(new BoolCommonExpression.First(traversed.BooleanValue));
                }
                else if (expression is MemberExpression memberExpression)
                {
                    var traversed = TraverseMemberExpression(memberExpression);
                    if (traversed.PrimitiveLiteral != null &&
                        traversed.PrimitiveLiteral is PrimitiveLiteral.BooleanValueNode booleanValueNode)
                    {
                        return new Filter(new BoolCommonExpression.First(booleanValueNode.BooleanValue));
                    }
                    else if (traversed.BoolFirstMemberExpression != null)
                    {
                        return new Filter(new BoolCommonExpression.Third(traversed.BoolFirstMemberExpression));
                    }
                    else
                    {
                        throw new Exception("TODO a c# member was accessed, but it was not a boolean literal and it was not a closure on a boolean local variable");
                    }
                }
                else if (expression is BinaryExpression binaryExpression)
                {
                    var traversed = TraverseBinaryExpression(binaryExpression);
                    if (traversed.Tenth != null)
                    {
                        return new Filter(traversed.Tenth);
                    }
                    if (traversed.Eleventh != null)
                    {
                        return new Filter(traversed.Eleventh);
                    }
                    if (traversed.Twelfth != null)
                    {
                        return new Filter(traversed.Twelfth);
                    }
                    if (traversed.Thirteenth != null)
                    {
                        return new Filter(traversed.Thirteenth);
                    }
                    if (traversed.Fourteenth != null)
                    {
                        return new Filter(traversed.Fourteenth);
                    }
                    if (traversed.Fifteenth != null)
                    {
                        return new Filter(traversed.Fifteenth);
                    }
                    else
                    {
                        throw new Exception("TODO a c# binary operation was provided, but it wasn't a boolean operation");
                    }
                }
                else
                {
                    throw new Exception("TODO");
                }
            }

            private static 
                (
                    BoolCommonExpression.Tenth? Tenth,
                    BoolCommonExpression.Eleventh? Eleventh,
                    BoolCommonExpression.Twelfth? Twelfth,
                    BoolCommonExpression.Thirteenth? Thirteenth,
                    BoolCommonExpression.Fourteenth? Fourteenth,
                    BoolCommonExpression.Fifteenth? Fifteenth,
                    object? NonboolBinary
                )
                TraverseBinaryExpression(BinaryExpression expression)
            {
                var leftTraversed = TraverseExpression(expression.Left);
                if (leftTraversed.PrimitiveLiteral == null)
                {
                    throw new Exception("TODO implement the rest of the linq expression traversal");
                }

                var rightTraversed = TraverseExpression(expression.Right);
                if (rightTraversed.CommonExpression == null)
                {
                    throw new Exception("TODO implement the rest of the linq expression traversal");
                }

                if (
                    (expression.Method?.IsSpecialName == true && expression.Method?.Name == "op_Equality") // 'operator ==' overload
                    || (expression.NodeType == ExpressionType.Equal) // primitive equality provided by the compiler
                    )
                {
                    return
                        (
                            new BoolCommonExpression.Tenth(
                                leftTraversed.PrimitiveLiteral,
                                new EqualsExpression(rightTraversed.CommonExpression)),
                            null,
                            null,
                            null,
                            null,
                            null,
                            null
                        );
                }
                else if (
                    (expression.Method?.IsSpecialName == true && expression.Method?.Name == "op_GreaterThan") // 'operator >' overload
                    || (expression.NodeType == ExpressionType.GreaterThan) // primitive comparison provided by the compiler
                    )
                {
                    return
                        (
                            null,
                            null,
                            null,
                            null,
                            new BoolCommonExpression.Fourteenth(
                                leftTraversed.PrimitiveLiteral,
                                new GreaterThanExpression(rightTraversed.CommonExpression)),
                            null,
                            null
                        );
                }
                else
                {
                    //// TODO other operators
                    throw new Exception("TODO");
                }
            }

            private static (PrimitiveLiteral? PrimitiveLiteral, CommonExpression? CommonExpression, object? Other) TraverseExpression(Expression expression)
            {
                if (expression is ConstantExpression constantExpression)
                {
                    var traversed = TraverseConstantExpression(constantExpression);
                    if (traversed.PrimitiveLiteral != null)
                    {
                        return (traversed.PrimitiveLiteral, null, null);
                    }
                    else
                    {
                        throw new Exception("TODO");
                    }
                }
                else if (expression is MemberExpression memberExpression)
                {
                    var traversed = TraverseMemberExpression(memberExpression);
                    if (traversed.PrimitiveLiteral != null)
                    {
                        return (traversed.PrimitiveLiteral, null, null);
                    }
                    else if (traversed.BoolFirstMemberExpression != null)
                    {
                        throw new Exception("TODO"); //// TODO convert to common expression
                    }
                    else if (traversed.NonboolMemberExpression != null)
                    {
                        return (null, traversed.NonboolMemberExpression, null);
                    }
                    else
                    {
                        throw new Exception("TODO");
                    }
                }
                else
                {
                    throw new Exception("TODO");
                }
            }

            private static (BooleanValue? BooleanValue, PrimitiveLiteral? PrimitiveLiteral)
                TraverseConstantExpression(ConstantExpression expression)
            {
                //// TODO nullable value in each of these branches?
                if (expression.Type == typeof(bool))
                {
                    BooleanValue booleanValue =
                        (bool)expression.Value! ? new BooleanValue.True() : new BooleanValue.False();
                    return (booleanValue, new PrimitiveLiteral.BooleanValueNode(booleanValue));
                }
                else if (expression.Type == typeof(string))
                {
                    return (null, new PrimitiveLiteral.StringValue((string)expression.Value!));
                }
                else if (expression.Type == typeof(DateTime))
                {
                    return (null, new PrimitiveLiteral.DateTimeOffsetValue((DateTime)expression.Value!));
                }
                else
                {
                    throw new Exception("TODO implement other constant expression translsations");
                }
            }

            private static (PrimitiveLiteral? PrimitiveLiteral, BoolFirstMemberExpression? BoolFirstMemberExpression, CommonExpression? NonboolMemberExpression)
                TraverseMemberExpression(MemberExpression expression)
            {
                var parameterAccess = TraverseParameterAccess(expression, Enumerable.Empty<MemberExpression>());
                if (parameterAccess.BoolFirstMemberExpression != null)
                {
                    return (null, parameterAccess.BoolFirstMemberExpression, null);
                }
                else if (parameterAccess.NonboolMemberExpression != null)
                {
                    return (null, null, parameterAccess.NonboolMemberExpression);
                }

                var closure = TraverseClosure(expression);
                return (closure, null, null);
            }

            private static (BoolFirstMemberExpression? BoolFirstMemberExpression, CommonExpression? NonboolMemberExpression) TraverseParameterAccess(MemberExpression expression, IEnumerable<MemberExpression> previousExpressions)
            {
                if (expression.Expression?.NodeType == ExpressionType.Constant)
                {
                    return (null, null);
                }
                else if (expression.Expression?.NodeType != ExpressionType.Parameter)
                {
                    if (expression.Expression is MemberExpression memberExpression)
                    {
                        if (memberExpression.Type.IsGenericType && memberExpression.Type.GetGenericTypeDefinition() == typeof(OdataInstanceProperty<>))
                        {
                        }
                        else
                        {
                            previousExpressions = previousExpressions.Append(expression);
                        }

                        return TraverseParameterAccess(memberExpression, previousExpressions);
                    }
                    else
                    {
                        throw new Exception("TODO i don't think you can actually get here");
                    }
                }
                else
                {
                    using (var enumerator = previousExpressions.Prepend(expression).GetEnumerator())
                    {
                        enumerator.MoveNext();

                        var traversed = TraversePreviousMembers(enumerator);
                        if (traversed.BoolMemberExpression != null)
                        {
                            return
                                (
                                    new BoolFirstMemberExpression.BoolMemberExpressionNode(
                                        traversed.BoolMemberExpression),
                                    null
                                );
                        }
                        else if (traversed.NonboolMemberExpression != null)
                        {
                            return
                                (
                                    null,
                                    traversed.NonboolMemberExpression
                                );
                        }
                        else
                        {
                            throw new Exception("TODO implement non-bool member access");
                        }
                    }
                }
            }

            private static (BoolMemberExpression? BoolMemberExpression, CommonExpression? NonboolMemberExpression) TraversePreviousMembers(IEnumerator<MemberExpression> expressions)
            {
                var expression = expressions.Current;
                var propertyNames = GetPropertyNames(expression.Member.DeclaringType!); //// TODO nullable
                if (propertyNames.Contains(expression.Member.Name))
                {
                    var translatedName = expression.Member.Name;
                    var propertyNameAttribute = expression.Member.GetCustomAttribute<PropertyNameAttribute>(false);
                    if (propertyNameAttribute != null)
                    {
                        translatedName = propertyNameAttribute.PropertyName;
                    }

                    if (!expressions.MoveNext())
                    {
                        if (expression.Type == typeof(bool) || expression.Type == typeof(OdataInstanceProperty<bool>))
                        {
                            return
                                (
                                    new BoolMemberExpression.Unqualified.PropertyPath(
                                        new BoolPropertyPathExpression.PrimitveNode.Primitive(
                                            new PrimitiveProperty.PrimitiveNonKeyProperty(
                                                new OdataIdentifier(translatedName)))),
                                    null
                                );
                        }
                        else
                        {
                            return
                                (
                                    null,
                                    new CommonExpression.TodoTerminal(
                                        new OdataIdentifier(translatedName))
                                );
                        }
                    }
                    else
                    {
                        var traversed = TraversePreviousMembers(expressions);

                        if (traversed.BoolMemberExpression != null)
                        {
                            return
                                (
                                    new BoolMemberExpression.Unqualified.PropertyPath(
                                        new BoolPropertyPathExpression.PrimitveNode.Entity(
                                            new OdataIdentifier(translatedName),
                                            new BoolSingleNavigationExpression(
                                                traversed.BoolMemberExpression))),
                                    null
                                );
                        }
                        else if (traversed.NonboolMemberExpression != null)
                        {
                            return
                                (
                                    null,
                                    new CommonExpression.Todo(
                                        new OdataIdentifier(translatedName),
                                        traversed.NonboolMemberExpression)
                                );
                        }
                        else
                        {
                            throw new Exception("TODO");
                        }
                    }

                }
                else
                {
                    throw new Exception("TODO property name not found; you could get here if the memberexpression was manually instantiated or if the type has members defined that are not marked as odata properties");
                }
            }

            private static PrimitiveLiteral? TraverseClosure(MemberExpression expression)
            {
                if (expression.Expression is ConstantExpression constantExpression) //// this is a "dynamic" closure (both instance accesses, and locally scoped variable accesses)
                {
                    //// TODO despite what the above comment says, this actually only supports locally scoped variables

                    //// TODO null checks
                    var fieldInfo = constantExpression.Value?.GetType().GetField(expression.Member.Name);
                    var value = fieldInfo?.GetValue(constantExpression.Value);

                    if (fieldInfo?.FieldType == typeof(DateTime))
                    {
                        ////queryParameter.Append($"'{((DateTime)value!).ToString("yyyy-MM-ddThh:mm:ss.000000")}'");

                        //// TODO null check
                        var dateTimeOffset = new DateTimeOffset((DateTime)value!);
                        return new PrimitiveLiteral.DateTimeOffsetValue(dateTimeOffset);
                    }
                    else
                    {
                        //// TODO support other EDM primitives
                        throw new Exception("tODO");
                    }

                    //// TODO do you want to do a manual formatting of dateTimeConstantExpression.Value? do you want to actually call datetime.parse on it?

                    //// TODO use jsonpropertyname attributes to get the start and datetime strings (you should actually use brand new attributes)
                    ////return $"start/dateTime gt '{dateTimeValue}'";
                }
                else
                {
                    //// TODO support other kinds of closures here
                    throw new Exception("tODO");
                }
            }
        }
    }
}
