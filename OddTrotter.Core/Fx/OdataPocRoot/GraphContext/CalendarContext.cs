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
    using Fx.OdataPocRoot.GraphClient;
    using Fx.OdataPocRoot.Odata;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Filter;
    using Fx.OdataPocRoot.Odata.UriExpressionNodes.Select;
    using Fx.OdataPocRoot.Odata.UriExpressionVisitorImplementations;
    using Microsoft.VisualBasic;
    using OddTrotter.GraphClient;

    public sealed class CalendarContext : IInstanceContext<Calendar>
    {
        private readonly IGraphClient graphClient;

        private readonly RelativeUri calendarUri;

        private readonly SelectToStringVisitor selectToStringVisitor;

        private readonly FilterToStringVisitor filterToStringVisitor;

        private readonly Select? select;

        public CalendarContext(
            IGraphClient graphClient,
            RelativeUri calendarUri,
            SelectToStringVisitor selectToStringVisitor,
            FilterToStringVisitor filterToStringVisitor) //// TODO it's pretty weird that you need a filter visitor for an instance context
            : this(
                  graphClient,
                  calendarUri,
                  selectToStringVisitor,
                  filterToStringVisitor,
                  null)
        {
        }

        private CalendarContext(
            IGraphClient graphClient, 
            RelativeUri calendarUri,
            SelectToStringVisitor selectToStringVisitor,
            FilterToStringVisitor filterToStringVisitor,
            Select? select)
        {
            this.graphClient = graphClient;
            this.calendarUri = calendarUri;
            this.selectToStringVisitor = selectToStringVisitor;
            this.filterToStringVisitor = filterToStringVisitor;
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
            //// TODO what about multiple selects on the same property?
            var select = LinqToOdata.Select(selector);
            if (this.select != null)
            {
                select = new Select(this.select.SelectItems.Concat(select.SelectItems));
            }

            return new CalendarContext(
                this.graphClient, 
                this.calendarUri,
                this.selectToStringVisitor, 
                this.filterToStringVisitor,
                select);
        }

        public IInstanceContext<TProperty> SubContext<TProperty>(Expression<Func<Calendar, OdataInstanceProperty<TProperty>>> selector)
        {
            throw new NotImplementedException();
        }

        public ICollectionContext<TProperty> SubContext<TProperty>(Expression<Func<Calendar, OdataCollectionProperty<TProperty>>> selector)
        {
            if (selector.Body is MemberExpression memberExpression)
            {
                if (memberExpression.Expression?.NodeType == ExpressionType.Parameter)
                {
                    var translatedName = memberExpression.Member.Name;
                    var propertyNameAttribute = memberExpression.Member.GetCustomAttribute<PropertyNameAttribute>();
                    if (propertyNameAttribute != null)
                    {
                        translatedName = propertyNameAttribute.PropertyName;
                    }

                    //// TODO generalize this
                    if (string.Equals(translatedName, "events"))
                    {
                        return (new EventsContext(
                            this.graphClient, 
                            new Uri(this.calendarUri.OriginalString.TrimEnd('/') + "/events", UriKind.Relative).ToRelativeUri(), 
                            this.selectToStringVisitor,
                            this.filterToStringVisitor
                            ) as ICollectionContext<TProperty>)!; //// TODO nullable
                    }
                    else
                    {
                        throw new Exception($"TODO subcontext not supported for {translatedName}");
                    }
                }
                else
                {
                    throw new Exception("TODO only direct member accesses supported");
                }
            }
            else
            {
                throw new Exception("TODO only direct member accesses supported");
            }
        }

        /// <summary>
        /// TODO is it good to have this be nested and public? i like it public so taht people don't have to go through a calendar to get to events if they know the direct url
        /// </summary>
        public sealed class EventsContext : ICollectionContext<Event>
        {
            private readonly IGraphClient graphClient;

            private readonly RelativeUri eventsUri;

            private readonly SelectToStringVisitor selectToStringVisitor;

            private readonly FilterToStringVisitor filterToStringVisitor;

            private readonly Select? select;

            private readonly Filter? filter;

            public EventsContext(
                IGraphClient graphClient,
                RelativeUri eventsUri, 
                SelectToStringVisitor selectToStringVisitor,
                FilterToStringVisitor filterToStringVisitor)
                : this(
                      graphClient, 
                      eventsUri,
                      selectToStringVisitor, 
                      filterToStringVisitor,
                      null,
                      null)
            {
            }

            public EventsContext(
                IGraphClient graphClient, 
                RelativeUri eventsUri, 
                SelectToStringVisitor selectToStringVisitor, 
                FilterToStringVisitor filterToStringVisitor,
                Select? select,
                Filter? filter)
            {
                this.graphClient = graphClient;
                this.eventsUri = eventsUri;
                this.selectToStringVisitor = selectToStringVisitor;
                this.filterToStringVisitor = filterToStringVisitor;
                this.select = select;
                this.filter = filter;
            }

            public async Task<OdataCollection<Event>> Evaluate()
            {
                var queryOptions = new List<string>();
                if (this.select != null)
                {
                    var stringBuilder = new StringBuilder();
                    this.selectToStringVisitor.Visit(this.select, stringBuilder);
                    queryOptions.Add(stringBuilder.ToString());
                }

                if (this.filter != null)
                {
                    var stringBuilder = new StringBuilder();
                    this.filterToStringVisitor.Visit(this.filter, stringBuilder);
                    queryOptions.Add(stringBuilder.ToString());
                }

                var optionsString = string.Join("&", queryOptions);

                var requestUri =
                    this.eventsUri.OriginalString.TrimEnd('/') +
                    (string.IsNullOrEmpty(optionsString) ? string.Empty : $"?{optionsString}");

                var jsonSerializerOptions = new JsonSerializerOptions();
                jsonSerializerOptions.TypeInfoResolver = new TypeInfoResolver();
                jsonSerializerOptions.Converters.Add(new ConverterFactory());
                jsonSerializerOptions.Converters.Add(new CollectionConverterFactory());
                var deserializedResponse = await this.graphClient.GetOdataCollection<Event>(new Uri(requestUri, UriKind.Relative).ToRelativeUri(), jsonSerializerOptions).ConfigureAwait(false); 

                return deserializedResponse;
            }

            public ICollectionContext<Event> Filter(Expression<Func<Event, bool>> predicate)
            {
                var filter = LinqToOdata.Filter(predicate);

                //// TODO do an "and" when there's already a filter

                return new EventsContext(
                    this.graphClient,
                    this.eventsUri,
                    this.selectToStringVisitor,
                    this.filterToStringVisitor,
                    this.select,
                    filter);
            }

            public ICollectionContext<Event> OrderBy<TProperty>(Expression<Func<Event, TProperty>> selector)
            {
                throw new NotImplementedException();
            }

            public ICollectionContext<Event> Select<TProperty>(Expression<Func<Event, TProperty>> selector)
            {
                //// TODO what about multiple selects on the same property?
                var select = LinqToOdata.Select(selector);
                if (this.select != null)
                {
                    select = new Select(this.select.SelectItems.Concat(select.SelectItems));
                }

                return new EventsContext(
                    this.graphClient, 
                    this.eventsUri,
                    this.selectToStringVisitor, 
                    this.filterToStringVisitor,
                    select, 
                    this.filter);
            }

            public ICollectionContext<Event> Top(int count)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class TypeInfoResolver : IJsonTypeInfoResolver
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
                        //// TODO properly handle no matches
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
}
