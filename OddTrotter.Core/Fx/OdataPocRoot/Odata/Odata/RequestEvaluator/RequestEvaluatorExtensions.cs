////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
namespace Fx.OdataPocRoot.Odata.Odata.RequestEvaluator
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.Json;
    using System.Text.Json.Serialization.Metadata;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;

    using Fx.OdataPocRoot.Graph;
    using System.IO;

    public static class RequestEvaluatorExtensions
    {
        public static async Task<OdataResponse<T>.GetInstance> Evaluate<T>(
            this IRequestEvaluator requestEvaluator,
            OdataRequest<T>.GetInstance request)
        {
            //// TODO use mixins for this? probably would be nice so callers can override serialization behavior
            
            var response = await requestEvaluator.Evaluate(request.Request).ConfigureAwait(false); //// TODO the stream inside here needs to be disposed

            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.TypeInfoResolver = new TypeInfoResolver();
            jsonSerializerOptions.Converters.Add(new ConverterFactory());
            jsonSerializerOptions.Converters.Add(new CollectionConverterFactory());
            //// TODO also deserialzie control information
            var instance = JsonSerializer.Deserialize<T>(response.Contents, jsonSerializerOptions);
            if (instance == null)
            {
                throw new System.Exception("TODO null instance");
            }

            return new OdataResponse<T>.GetInstance(
                instance, 
                new OdataResponse<T>.GetInstance.InstanceControlInformation());
        }

        public static async Task<OdataResponse<T>.GetCollection> Evaluate<T>(
            this IRequestEvaluator requestEvaluator,
            OdataRequest<T>.GetCollection request)
        {
            var response = await requestEvaluator.Evaluate(request.Request).ConfigureAwait(false);
            string responseString;
            using (var streamReader = new StreamReader(response.Contents))
            {
                responseString = streamReader.ReadToEnd();
            }

            var jsonSerializerOptions = new JsonSerializerOptions();
            jsonSerializerOptions.TypeInfoResolver = new TypeInfoResolver();
            jsonSerializerOptions.Converters.Add(new ConverterFactory());
            jsonSerializerOptions.Converters.Add(new CollectionConverterFactory());
            //// TODO also deserialzie control information
            var collection = JsonSerializer.Deserialize<ODataCollectionPage<T>.Builder>(
                responseString,
                jsonSerializerOptions); //// TODO do you liuke the odatacollectionpage?
            if (collection == null)
            {
                throw new System.Exception("TODO null collection");
            }

            return new OdataResponse<T>.GetCollection(
                collection.Value!, //// TODO nullable
                new OdataResponse<T>.GetCollection.CollectionControlInformation(null!, 0));
        }

        private sealed class ODataCollectionPage<T>
        {
            private ODataCollectionPage(IEnumerable<T> value, string? nextLink)
            {
                this.Value = value;
                this.NextLink = nextLink;
            }

            public IEnumerable<T> Value { get; }

            /// <summary>
            /// Gets the URL of the next page in the collection, <see langword="null"/> if there are no more pages in the collection
            /// </summary>
            public string? NextLink { get; }

            public sealed class Builder
            {
                [JsonPropertyName("value")]
                public IEnumerable<T>? Value { get; set; }

                [JsonPropertyName("@odata.nextLink")]
                public string? NextLink { get; set; }

                /// <summary>
                /// 
                /// </summary>
                /// <returns></returns>
                /// <exception cref="ArgumentNullException">Thrown if <see cref="Value"/> is <see langword="null"/></exception>
                public ODataCollectionPage<T> Build()
                {
                    if (this.Value == null)
                    {
                        throw new ArgumentNullException(nameof(this.Value));
                    }

                    return new ODataCollectionPage<T>(this.Value, this.NextLink);
                }
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
