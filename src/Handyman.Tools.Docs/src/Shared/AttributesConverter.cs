using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handyman.Tools.Docs.Shared;

public class AttributesConverter : IAttributesConverter
{
    private readonly IEnumerable<IValueParser> _valueParsers;

    public AttributesConverter(IEnumerable<IValueParser> valueParsers)
    {
        _valueParsers = valueParsers;
    }

    public TAttributes ConvertTo<TAttributes>(Attributes attributes) where TAttributes : new()
    {
        var set = attributes.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var tAttributes = new TAttributes();

        foreach (var property in tAttributes.GetType().GetProperties())
        {
            var name = property.Name;

            var nameAttribute = property.GetCustomAttribute<AttributeNameAttribute>();

            if (nameAttribute != null)
            {
                name = nameAttribute.Name;
            }

            if (!attributes.TryGet(name, out var stringValue))
                continue;

            set.Remove(name);

            if (property.PropertyType == typeof(string))
            {
                property.SetValue(tAttributes, stringValue);
                continue;
            }

            object value = null;
            if (_valueParsers.Any(x => x.TryParse(stringValue, property.PropertyType, out value)))
            {
                property.SetValue(tAttributes, value);
                continue;
            }

            throw new InvalidOperationException($"Unable to convert attributes to {typeof(TAttributes).Name}.");
        }

        if (set.Any())
        {
            throw new Exception($"todo - unhandled attributes; {string.Join(", ", set)}");
        }

        if (tAttributes is IValidatable validatable)
        {
            validatable.Validate();
        }

        return tAttributes;
    }
}