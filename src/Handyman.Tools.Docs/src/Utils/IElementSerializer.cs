using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Handyman.Tools.Docs.Utils
{
    public interface IElementsSerializer<TAttributes>
    {
        IEnumerable<Element<TAttributes>> ReadElement(string elementName, IReadOnlyList<string> lines);
        void WriteElement(Element<TAttributes> element, IEnumerable<string> content, List<string> lines);
    }

    public class ElementsSerializer<TAttributes> : IElementsSerializer<TAttributes>
    {
        private const string Pattern = "^(?<prefix>.*?)<(?<close>/)?handyman-docs:(?<name>[a-z0-9-._]+)( +(?<attributes>[a-z]+=\"[^\"]*\"))* *(?<selfClose>/)?>(?<suffix>.*)$";

        private readonly IAttributesParser<TAttributes> _attributesParser;

        public ElementsSerializer(IAttributesParser<TAttributes> attributesParser)
        {
            _attributesParser = attributesParser;
        }

        public IEnumerable<Element<TAttributes>> ReadElement(string elementName, IReadOnlyList<string> lines)
        {
            var regex = new Regex(Pattern, RegexOptions.IgnoreCase);

            var tags = new List<Tag>();

            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                var match = regex.Match(line);

                if (!match.Success)
                    continue;

                var tag = new Tag
                {
                    Attributes = ParseAttributes(match.Groups["attributes"].Captures.Select(x => x.Value)),
                    IsClosing = match.Groups["close"].Success,
                    IsSelfClosing = match.Groups["selfClose"].Success,
                    LineIndex = i,
                    Name = match.Groups["name"].Value,
                    Prefix = match.Groups["prefix"].Value,
                    Suffix = match.Groups["suffix"].Value
                };

                tags.Add(tag);
            }

            var elements = new List<Element<TAttributes>>();

            for (var i = 0; i < tags.Count; i++)
            {
                var tag = tags[i];
                var isLastTag = i == (tags.Count - 1);

                var closingLineIndex = 0;

                if (tag.IsSelfClosing)
                {
                    closingLineIndex = tag.LineIndex;
                }
                else if (tag.IsOpen)
                {
                    if (isLastTag)
                        throw new NotImplementedException("matching close tag not found");

                    var nextTag = tags[i + 1];

                    if (tag.Name != nextTag.Name || !nextTag.IsClosing)
                        throw new NotImplementedException("matching close tag not found");

                    closingLineIndex = nextTag.LineIndex;
                    i++;
                }
                else
                {
                    throw new NotImplementedException("found unexpected close tag");
                }

                if (elementName != null && tag.Name != elementName)
                    continue;

                if (!_attributesParser.TryParse(tag.Attributes, out var attributes))
                {
                    // todo
                }

                var lineCount = Math.Max(0, closingLineIndex - tag.LineIndex) + 1;

                elements.Add(new Element<TAttributes>
                {
                    Attributes = attributes,
                    ContentLineCount = lineCount <= 2 ? 0 : lineCount - 2,
                    ContentLineIndex = lineCount <= 2 ? 0 : tag.LineIndex + 1,
                    ElementLineIndex = tag.LineIndex,
                    ElementLineCount = lineCount,
                    Name = tag.Name,
                    Prefix = tag.Prefix,
                    Suffix = tag.Suffix
                });
            }

            return elements;
        }

        public void WriteElement(Element<TAttributes> element, IEnumerable<string> content, List<string> lines)
        {
            throw new NotImplementedException();
        }

        private static KeyValuePair<string, string>[] ParseAttributes(IEnumerable<string> strings)
        {
            return strings
                .Select(s =>
                {
                    var index = s.IndexOf("=", StringComparison.Ordinal);
                    var key = s.Substring(0, index);
                    var value = s.Substring(index + 2, s.Length - (index + 3));
                    return new KeyValuePair<string, string>(key, value);
                })
                .ToArray();
        }

        private class Tag
        {
            public KeyValuePair<string, string>[] Attributes { get; set; }
            public bool IsClosing { get; set; }
            public bool IsSelfClosing { get; set; }
            public int LineIndex { get; set; }
            public string Name { get; set; }
            public string Prefix { get; set; }
            public string Suffix { get; set; }

            public bool IsOpen => !IsClosing && !IsSelfClosing;
        }
    }

    public interface IAttributesParser<TAttributes>
    {
        bool TryParse(IReadOnlyList<KeyValuePair<string, string>> keyValuePairs, out TAttributes attributes);
    }

    public class AttributesParser<TAttributes> : IAttributesParser<TAttributes>
    {
        public bool TryParse(IReadOnlyList<KeyValuePair<string, string>> keyValuePairs, out TAttributes attributes)
        {
            attributes = (TAttributes)Activator.CreateInstance(typeof(TAttributes));

            if (!TryPopulate(attributes, keyValuePairs))
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
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public int ElementLineIndex { get; set; }
        public int ElementLineCount { get; set; }
        public int ContentLineIndex { get; set; }
        public int ContentLineCount { get; set; }

        public bool HasContent => ContentLineCount != 0;
    }

    public interface IAttributeConverter
    {
        bool TryConvertFromString(string s, ILogger logger, out object value);
        string ConvertToString(object value);
    }

    public abstract class AttributeConverter<TAttribute> : IAttributeConverter
    {
        bool IAttributeConverter.TryConvertFromString(string s, ILogger logger, out object value)
        {
            if (TryConvertFromString(s, logger, out var a))
            {
                value = a;
                return true;
            }

            value = null;
            return false;
        }

        public abstract bool TryConvertFromString(string s, ILogger logger, out TAttribute value);

        string IAttributeConverter.ConvertToString(object value)
        {
            return ConvertToString((TAttribute)value);
        }

        public abstract string ConvertToString(TAttribute value);
    }

    public class DefaultAttributeConverter<T> : AttributeConverter<T>
    {
        public override bool TryConvertFromString(string s, ILogger logger, out T value)
        {
            value = (T)Convert.ChangeType(s, typeof(T));
            return true;
        }

        public override string ConvertToString(T value)
        {
            return (string)Convert.ChangeType(value, typeof(string));
        }
    }

    public class LinesAttributeConverter : AttributeConverter<Lines>
    {
        public override bool TryConvertFromString(string s, ILogger logger, out Lines value)
        {
            value = null;

            if (int.TryParse(s, out var from))
            {
                if (from < 1)
                {
                    logger.Log($"Invalid format '{s}', value can't be less than 1.");
                    return false;
                }

                value = new Lines
                {
                    Count = 1,
                    From = from,
                    Text = s
                };

                return true;
            }

            if (TryParse(s, '-', out from, out var to))
            {
                if (from < 1)
                {
                    logger.Log($"Invalid format '{s}', from can't be less than 1.");
                    return false;
                }

                if (to < @from)
                {
                    logger.Log($"Invalid format '{s}', from can't greater than to.");
                    return false;
                }

                value = new Lines
                {
                    Count = (to - from) + 1,
                    From = from,
                    Text = s
                };

                return true;
            }

            if (TryParse(s, '+', out from, out var count))
            {
                if (from < 1)
                {
                    logger.Log($"Invalid format '{s}', from can't be less than 1.");
                    return false;
                }

                if (count < 1)
                {
                    logger.Log($"Invalid format '{s}', count can't be less than 1.");
                    return false;
                }

                value = new Lines
                {
                    Count = count + 1,
                    From = from,
                    Text = s
                };

                return true;
            }

            logger.Log($"Invalid format '{s}'.");
            return false;
        }

        private static bool TryParse(string s, char separator, out int first, out int second)
        {
            first = 0;
            second = 0;

            if (s.Count(x => x == separator) != 1)
                return false;

            var index = s.IndexOf(separator);

            if (index <= 0 || index == s.Length - 1)
                return false;

            var s1 = s.Substring(0, index);
            var s2 = s.Substring(index + 1);

            return int.TryParse(s1, out first) && int.TryParse(s2, out second);
        }

        public override string ConvertToString(Lines value)
        {
            return value.Text;
        }
    }

    public class Lines
    {
        public int From { get; set; }
        public int Count { get; set; }
        public string Text { get; set; }

        public int FromIndex => From - 1;
    }

    public interface ILogger
    {
        void Log(string message);
    }

    public class Logger : ILogger
    {
        public List<string> Messages { get; } = new List<string>();

        public void Log(string message)
        {
            Messages.Add(message);
        }
    }

    public class RequiredAttribute : Attribute { }

    public class XorAttribute : Attribute { }
}