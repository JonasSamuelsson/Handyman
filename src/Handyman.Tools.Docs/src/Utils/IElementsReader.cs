using System;
using System.Collections.Generic;

namespace Handyman.Tools.Docs.Utils
{
    public interface IElementsReader<TAttributes>
    {
        IEnumerable<Element<TAttributes>> Read(IReadOnlyCollection<string> lines);
    }

    public class ElementsReader<TAttributes> : IElementsReader<TAttributes>
    {
        private readonly IAttributesParser<TAttributes> _attributesParser;

        public ElementsReader(IAttributesParser<TAttributes> attributesParser)
        {
            _attributesParser = attributesParser;
        }

        public IEnumerable<Element<TAttributes>> Read(IReadOnlyCollection<string> lines)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IAttributesParser<TAttributes>
    {
        bool TryParse(Dictionary<string, string> dictionary, out TAttributes attributes);
    }

    public class AttributesParser<TAttributes> : IAttributesParser<TAttributes>
    {
        public bool TryParse(Dictionary<string, string> dictionary, out TAttributes attributes)
        {
            attributes = (TAttributes)Activator.CreateInstance(typeof(TAttributes));

            if (!TryPopulate(attributes, dictionary))
            {
                attributes = default;
                return false;
            }

            return true;
        }

        protected virtual bool TryPopulate(TAttributes attributes, Dictionary<string, string> dictionary)
        {
            throw new NotImplementedException();
        }
    }

    public class Element<TAttributes>
    {
        public TAttributes Attributes { get; set; }
        public string Name { get; set; }
        public bool IsSelfClosed { get; set; }
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public int ElementLineIndex { get; set; }
        public int ElementLineCount { get; set; }
        public int ContentLineIndex { get; set; }
        public int ContentLineCount { get; set; }
    }

    public interface IAttributeConverter
    {
        bool TryConvertFromString(string s, out object attribute);
        string ConvertToString(object attribute);
    }

    public abstract class AttributeConverter<TAttribute> : IAttributeConverter
    {
        bool IAttributeConverter.TryConvertFromString(string s, out object attribute)
        {
            if (TryConvertFromString(s, out var a))
            {
                attribute = a;
                return true;
            }

            attribute = null;
            return false;
        }

        public abstract bool TryConvertFromString(string s, out TAttribute attribute);

        string IAttributeConverter.ConvertToString(object attribute)
        {
            return ConvertToString((TAttribute)attribute);
        }

        public abstract string ConvertToString(TAttribute attribute);
    }
}