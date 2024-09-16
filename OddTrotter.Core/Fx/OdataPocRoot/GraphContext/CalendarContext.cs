namespace Fx.OdataPocRoot.GraphContext
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Metadata.Ecma335;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Text.Json.Serialization.Metadata;
    using System.Threading.Tasks;

    using Fx.OdataPocRoot.Graph;
    using Fx.OdataPocRoot.Odata;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Select;
    using Fx.OdataPocRoot.Odata.UriExpressionVisitorImplementations;
    using Microsoft.VisualBasic;
    using OddTrotter.GraphClient;

    public sealed class CalendarContext : IInstanceContext<Calendar>
    {
        private readonly IGraphClient graphClient;

        private readonly RelativeUri calendarUri;

        private readonly SelectToStringVisitor selectToStringVisitor;

        private readonly Select? select;

        public CalendarContext(IGraphClient graphClient, RelativeUri calendarUri, SelectToStringVisitor selectToStringVisitor)
            : this(graphClient, calendarUri, selectToStringVisitor, null)
        {
        }

        private CalendarContext(IGraphClient graphClient, RelativeUri calendarUri, SelectToStringVisitor selectToStringVisitor, Select? select)
        {
            this.graphClient = graphClient;
            this.calendarUri = calendarUri;
            this.selectToStringVisitor = selectToStringVisitor;
            this.select = select;
        }

        public async Task<Calendar> Evaluate()
        {
            var queryOptions = new List<string>();
            if (this.select != null)
            {
                var stringBuilder = new StringBuilder();
                this.selectToStringVisitor.Visit(this.select, stringBuilder);
                queryOptions.Add(stringBuilder.ToString());
            }

            var optionsString = string.Join("&", queryOptions);

            var requestUri = 
                this.calendarUri.OriginalString.TrimEnd('/') + 
                (string.IsNullOrEmpty(optionsString) ? string.Empty : $"?{optionsString}");

            using (var httpResponseMessage = await this.graphClient.GetAsync(new Uri(requestUri, UriKind.Relative).ToRelativeUri()).ConfigureAwait(false))
            {
                httpResponseMessage.EnsureSuccessStatusCode();
                var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                var jsonSerializerOptions = new JsonSerializerOptions();
                jsonSerializerOptions.TypeInfoResolver = new TypeInfoResolver();
                jsonSerializerOptions.Converters.Add(new ConverterFactory());
                jsonSerializerOptions.Converters.Add(new CollectionConverterFactory());
                var calendar = JsonSerializer.Deserialize<Calendar>(httpResponseContent, jsonSerializerOptions);
                if (calendar == null)
                {
                    throw new Exception("TODO null calendar");
                }

                return calendar;
            }
        }

        public IInstanceContext<Calendar> Select<TProperty>(Expression<Func<Calendar, TProperty>> selector)
        {
            var select = LinqToOdata.Select(selector);
            if (this.select != null)
            {
                select = new Select(this.select.SelectItems.Concat(select.SelectItems));
            }

            return new CalendarContext(this.graphClient, this.calendarUri, this.selectToStringVisitor, select);
        }

        public IInstanceContext<TProperty> SubContext<TProperty>(Expression<Func<Calendar, OdataInstanceProperty<TProperty>>> selector)
        {
            throw new NotImplementedException();
        }

        public ICollectionContext<TProperty> SubContext<TProperty>(Expression<Func<Calendar, OdataCollectionProperty<TProperty>>> selector)
        {
            throw new NotImplementedException();
        }

        public sealed class TypeInfoResolver : IJsonTypeInfoResolver
        {
            public JsonTypeInfo? GetTypeInfo(Type type, JsonSerializerOptions options)
            {                
                var defaultJsonTypeInfoResolver = new DefaultJsonTypeInfoResolver();
                var jsonTypeInfo = defaultJsonTypeInfoResolver.GetTypeInfo(type, options);

                for (int i = 0; i < jsonTypeInfo.Properties.Count; ++i)
                {
                    var members = type.GetMember(jsonTypeInfo.Properties[i].Name);
                    MemberInfo member;
                    try
                    {
                        member = members.Single();
                    }
                    catch
                    {
                        //// TODO property handle no matches
                        //// TODO properly handle multiple matches
                        return jsonTypeInfo;
                    }

                    var translatedName = member.Name;
                    var propertyNameAttribute = member.GetCustomAttribute<PropertyNameAttribute>(false);
                    if (propertyNameAttribute != null)
                    {
                        translatedName = propertyNameAttribute.PropertyName;
                    }

                    jsonTypeInfo.Properties[i].Name = translatedName;
                }

                return jsonTypeInfo;
            }
        }
        private sealed class CollectionConverterFactory : JsonConverterFactory
        {
            public override bool CanConvert(Type typeToConvert)
            {
                return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(OdataCollectionProperty<>);
            }

            public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            {
                var typeArgument = typeToConvert.GenericTypeArguments[0];
                var converterType = typeof(CollectionConverter<>);
                var genericConverterType = converterType.MakeGenericType(typeArgument);

                var createdConverter = Activator.CreateInstance(genericConverterType);
                var converter = createdConverter as JsonConverter;

                return converter;
            }
        }

        private sealed class CollectionConverter<TProperty> : JsonConverter<OdataCollectionProperty<TProperty>>
        {
            public override OdataCollectionProperty<TProperty>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var propertyValue = JsonSerializer.Deserialize<IEnumerable<TProperty>>(ref reader, options);
                var odataProperty = new OdataCollectionProperty<TProperty>(propertyValue!);
                return odataProperty;
            }

            public override void Write(Utf8JsonWriter writer, OdataCollectionProperty<TProperty> value, JsonSerializerOptions options)
            {
                //// TODO implement this?
                throw new NotImplementedException();
            }
        }

        private sealed class ConverterFactory : JsonConverterFactory
        {
            public override bool CanConvert(Type typeToConvert)
            {
                return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(OdataInstanceProperty<>);
            }

            public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
            {
                var typeArgument = typeToConvert.GenericTypeArguments[0];
                var converterType = typeof(Converter<>);
                var genericConverterType = converterType.MakeGenericType(typeArgument);

                var createdConverter = Activator.CreateInstance(genericConverterType);
                var converter = createdConverter as JsonConverter;

                return converter;
            }
        }

        private sealed class Converter<TProperty> : JsonConverter<OdataInstanceProperty<TProperty>>
        {
            public override OdataInstanceProperty<TProperty>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var propertyValue = JsonSerializer.Deserialize<TProperty>(ref reader, options);
                var odataProperty = new OdataInstanceProperty<TProperty>(propertyValue!);
                return odataProperty;
            }

            public override void Write(Utf8JsonWriter writer, OdataInstanceProperty<TProperty> value, JsonSerializerOptions options)
            {
                //// TODO implement this?
                throw new NotImplementedException();
            }
        }
    }

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
                var propertyNameAttribute = expression.Member.GetCustomAttribute<PropertyNameAttribute>();
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
