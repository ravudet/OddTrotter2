namespace Fx.OdataPocRoot.GraphContext
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection.Metadata.Ecma335;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    using Fx.OdataPocRoot.Graph;
    using Fx.OdataPocRoot.Odata;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Common;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Select;
    using Microsoft.VisualBasic;
    using OddTrotter.GraphClient;

    public sealed class CalendarContext : IInstanceContext<Calendar>
    {
        private readonly IGraphClient graphClient;

        private readonly RelativeUri calendarUri;

        private readonly Select? select;

        public CalendarContext(IGraphClient graphClient, RelativeUri calendarUri)
            : this(graphClient, calendarUri, null)
        {
        }

        private CalendarContext(IGraphClient graphClient, RelativeUri calendarUri, Select? select)
        {
            this.graphClient = graphClient;
            this.calendarUri = calendarUri;
            this.select = select;
        }

        public async Task<Calendar> Evaluate()
        {
            //// TODO generate string from select and append it to uri
            using (var httpResponseMessage = await this.graphClient.GetAsync(this.calendarUri).ConfigureAwait(false))
            {
                httpResponseMessage.EnsureSuccessStatusCode();
                var httpResponseContent = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

                var jsonSerializerOptions = new JsonSerializerOptions();
                jsonSerializerOptions.Converters.Add(new ConverterFactory());
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

            return new CalendarContext(this.graphClient, this.calendarUri, select);
        }

        private sealed class ConverterFactory : JsonConverterFactory
        {
            public override bool CanConvert(Type typeToConvert)
            {
                return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(OdataProperty<>);
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

        private sealed class Converter<TProperty> : JsonConverter<OdataProperty<TProperty>>
        {
            public override OdataProperty<TProperty>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var propertyValue = JsonSerializer.Deserialize<TProperty>(ref reader, options);
                var odataProperty = new OdataProperty<TProperty>(propertyValue!);
                return odataProperty;
            }

            public override void Write(Utf8JsonWriter writer, OdataProperty<TProperty> value, JsonSerializerOptions options)
            {
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
                if (!expressions.MoveNext())
                {
                    return
                        new SelectProperty.PrimitiveProperty(
                            new PrimitiveProperty.PrimitiveNonKeyProperty(
                                new OdataIdentifier(expression.Member.Name)
                            )
                        );
                }
                else
                {
                    return
                        new SelectProperty.FullSelectPath.SelectPropertyNode(
                            new SelectPath.First(
                                new OdataIdentifier(expression.Member.Name)
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
            var odataProperties = type.GetProperties().Where(property =>
                {
                    var propertyType = property.PropertyType;
                    if (!propertyType.IsGenericType)
                    {
                        return false;
                    }

                    if (propertyType.GetGenericTypeDefinition() != typeof(OdataProperty<>))
                    {
                        return false;
                    }

                    return true;
                })
                .Select(property => property.Name);

            return odataProperties;

            /*if (type == typeof(Calendar))
            {
                yield return nameof(Calendar.Id);
                yield return nameof(Calendar.Events);
                yield return nameof(Calendar.Foo);
            }
            else if (type == typeof(Foo))
            {
                yield return nameof(Foo.Bar);
            }
            else if (type == typeof(Bar))
            {
                yield return nameof(Bar.Test);
            }
            else if (type == typeof(Event))
            {
                yield return nameof(Event.Id);
                yield return nameof(Event.Body);
                yield return nameof(Event.End);
                yield return nameof(Event.IsCancelled);
                yield return nameof(Event.ResponseStatus);
                yield return nameof(Event.Start);
                yield return nameof(Event.Subject);
                yield return nameof(Event.Type);
                yield return nameof(Event.WebLink);
            }
            else if (type == typeof(ItemBody))
            {
                yield return nameof(ItemBody.Content);
            }
            else if (type == typeof(DateTimeTimeZone))
            {
                yield return nameof(DateTimeTimeZone.DateTime);
                yield return nameof(DateTimeTimeZone.TimeZone);
            }
            else if (type == typeof(ResponseStatus))
            {
                yield return nameof(ResponseStatus.Response);
                yield return nameof(ResponseStatus.Time);
            }
            else
            {
                throw new Exception("TODO actually implement this in a general way");
            }*/
        }
    }
}
