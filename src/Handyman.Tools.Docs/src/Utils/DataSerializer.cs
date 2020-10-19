using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handyman.Tools.Docs.Utils
{
    public class DataSerializer<TData> : IDataSerializer<TData>
    {
        private static readonly StringComparer StringComparerOrdinalIgnoreCase = StringComparer.OrdinalIgnoreCase;

        public bool TryDeserialize(IReadOnlyList<KeyValuePair<string, string>> keyValuePairs, ILogger logger, out TData data)
        {
            data = (TData)Activator.CreateInstance(typeof(TData));

            var groups = keyValuePairs
                .GroupBy(x => x.Key, StringComparerOrdinalIgnoreCase)
                .ToList();

            var success = true;

            foreach (var group in groups)
            {
                if (group.Count() == 1)
                    continue;

                logger.Log($"Duplicate '{group.Key}' attributes.");
                success = false;
            }

            if (!success)
            {
                return false;
            }

            var dictionary = keyValuePairs.ToDictionary(x => x.Key, x => x.Value, StringComparerOrdinalIgnoreCase);

            return TryDeserialize(dictionary, data, logger);
        }

        private static bool TryDeserialize(Dictionary<string, string> dictionary, TData data, ILogger logger)
        {
            var properties = typeof(TData).GetProperties();

            var xorProperties = properties
                .Where(x => x.GetCustomAttributes<XorAttribute>().Any())
                .Where(x => dictionary.ContainsKey(x.Name))
                .ToList();

            if (xorProperties.Count > 1)
            {
                var xorPropertyNames = xorProperties.Select(x => $"'{x.Name}'").OrderBy(x => x);
                logger.Log($"Attributes {string.Join(", ", xorPropertyNames)} can't be combined.");
                return false;
            }

            var success = true;

            foreach (var property in properties)
            {
                if (!dictionary.TryGetValue(property.Name, out var s))
                {
                    var required = property.GetCustomAttributes<RequiredAttribute>().Any();

                    if (required)
                    {
                        logger.Log($"Attribute '{property.Name}' is required but not provided.");
                        success = false;
                    }

                    continue;
                }

                var converter = GetValueConverter(property);

                using (logger.UsePrefix($"Attribute '{property.Name}' "))
                {
                    if (converter.TryConvertFromString(s, logger, out var value))
                    {
                        property.SetValue(data, value);
                    }
                }
            }

            return success;
        }

        public string Serialize(TData data)
        {
            var strings = new List<string>();

            // todo fix ordering
            foreach (var property in typeof(TData).GetProperties().OrderBy(x => x.Name))
            {
                var value = property.GetValue(data);

                if (value == null)
                    continue;

                var valueConverter = GetValueConverter(property);

                strings.Add($"{property.Name.ToLowerInvariant()}=\"{valueConverter.ConvertToString(value)}\"");
            }

            return strings.Count != 0
                ? $" {string.Join(" ", strings)}"
                : string.Empty;
        }

        private static IValueConverter GetValueConverter(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<ValueConverterAttribute>();
            var converterType = attribute?.ValueConverterType ?? typeof(DefaultValueConverter<>).MakeGenericType(property.PropertyType);
            return (IValueConverter)Activator.CreateInstance(converterType);
        }
    }
}