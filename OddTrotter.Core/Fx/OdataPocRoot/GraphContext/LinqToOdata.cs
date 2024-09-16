namespace Fx.OdataPocRoot.GraphContext
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using Fx.OdataPocRoot.Graph;
    using Fx.OdataPocRoot.Odata;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Select;

    /// <summary>
    /// TODO don't use a statics, use an interface instead?
    /// </summary>
    public static class LinqToOdata
    {
        public static Select Select<TType, TProperty>(Expression<Func<TType, TProperty>> selector)
        {
            if (selector.Body is MemberExpression memberExpression)
            {
                return TraverseSelect<TType>(memberExpression, Enumerable.Empty<MemberExpression>());
            }
            else
            {
                throw new Exception("TODO only member expressions are allowed");
            }
        }

        private static Select TraverseSelect<TType>(MemberExpression expression, IEnumerable<MemberExpression> previousExpressions)
        {
            if (expression.Expression?.NodeType != ExpressionType.Parameter)
            {
                if (expression.Expression is MemberExpression memberExpression)
                {
                    return TraverseSelect<TType>(memberExpression, previousExpressions.Prepend(expression));
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
                    return
                        new Select(
                            new[]
                            {
                                new SelectItem.PropertyPath.Second(
                                    TraversePreviousMembers(enumerator)
                                ),
                            });
                }
            }
        }

        private static SelectProperty TraversePreviousMembers(IEnumerator<MemberExpression> expressions)
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
                    return
                        new SelectProperty.PrimitiveProperty(
                            new PrimitiveProperty.PrimitiveNonKeyProperty(
                                new OdataIdentifier(translatedName)
                            )
                        );
                }
                else
                {
                    return
                        new SelectProperty.FullSelectPath.SelectPropertyNode(
                            new SelectPath.First(
                                new OdataIdentifier(translatedName)
                            ),
                            TraversePreviousMembers(expressions)
                        );
                }

            }
            else
            {
                throw new Exception("TODO property name not found; you could get here if the memberexpression was manually instantiated or if the type has members defined that are not marked as odata properties");
            }
        }

        private static IEnumerable<string> GetPropertyNames(Type type)
        {
            return type.GetProperties().Where(property =>
                {
                    var propertyType = property.PropertyType;
                    if (!propertyType.IsGenericType)
                    {
                        return false;
                    }

                    if (propertyType.GetGenericTypeDefinition() != typeof(OdataInstanceProperty<>) &&
                        propertyType.GetGenericTypeDefinition() != typeof(OdataCollectionProperty<>))
                    {
                        return false;
                    }

                    return true;
                })
                .Select(property => property.Name);
        }
    }
}
