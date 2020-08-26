using Newtonsoft.Json;
using System;

namespace Handyman.DuckTyping.Serialization
{
    internal class DuckTypedObjectJsonConverter : JsonConverter
    {
        public override bool CanRead => false;

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            serializer.Serialize(writer, ((DuckTypedObject)value).Dictionary);
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            do
            {
                if (objectType == typeof(DuckTypedObject))
                    return true;

                objectType = objectType.BaseType;
            } while (objectType != null);

            return false;
        }
    }
}