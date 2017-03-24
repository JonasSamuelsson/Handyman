using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Handyman.Dynamics
{
    internal class DObjectConverter : JsonConverter
    {
        private static readonly ExpandoObjectConverter ExpandoObjectConverter = new ExpandoObjectConverter();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var d = (DObject)value;
            serializer.Serialize(writer, d.Dictionary);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var @object = ExpandoObjectConverter.ReadJson(reader, objectType, existingValue, serializer);
            return DObject.Create((IEnumerable<KeyValuePair<string, object>>)@object);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DObject);
        }
    }
}