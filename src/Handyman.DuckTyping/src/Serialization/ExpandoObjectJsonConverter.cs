using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Handyman.DuckTyping.Serialization
{
    internal class ExpandoObjectJsonConverter : JsonConverter
    {
        public override bool CanRead => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();

            var dictionary = (IDictionary<string, object>)value;

            var propertiesToIgnore = SerializationInfo.GetPropertiesToIgnore(dictionary);

            var contractResolver = serializer.ContractResolver as DefaultContractResolver;

            foreach (var kvp in dictionary)
            {
                if (kvp.Key[0] == '_')
                {
                    continue;
                }

                if (propertiesToIgnore.Contains(kvp.Key))
                {
                    continue;
                }

                var propertyName = contractResolver?.GetResolvedPropertyName(kvp.Key) ?? kvp.Key;

                writer.WritePropertyName(propertyName);
                serializer.Serialize(writer, kvp.Value);
            }

            writer.WriteEndObject();
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ExpandoObject);
        }
    }
}