using Newtonsoft.Json;

namespace Handyman.DuckTyping.Serialization
{
    public static class JsonSerializerSettingsExtensions
    {
        public static JsonSerializerSettings AddDuckTyping(this JsonSerializerSettings settings)
        {
            settings.Converters.Insert(0, new DuckTypedObjectJsonConverter());
            settings.Converters.Insert(0, new ExpandoObjectJsonConverter());

            return settings;
        }
    }
}