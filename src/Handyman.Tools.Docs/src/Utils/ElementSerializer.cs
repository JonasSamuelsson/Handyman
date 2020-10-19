using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Handyman.Tools.Docs.Utils
{
    public class ElementSerializer<TData> : IElementSerializer<TData>
    {
        private const string Pattern = "^(?<prefix>.*?)<(?<close>/)?handyman-docs:(?<name>[a-z0-9-._]+)( +(?<attributes>[a-z]+=\"[^\"]*\"))* *(?<selfClose>/)?>(?<suffix>.*)$";

        private readonly IDataSerializer<TData> _dataSerializer;

        public ElementSerializer(IDataSerializer<TData> dataSerializer)
        {
            _dataSerializer = dataSerializer;
        }

        public IEnumerable<Element<TData>> TryDeserializeElements(string elementName, IReadOnlyList<string> lines, ILogger logger)
        {
            var regex = new Regex(Pattern, RegexOptions.IgnoreCase);

            var tags = new List<Tag>();

            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                var match = regex.Match(line);

                if (!match.Success)
                    continue;

                var attributes = ParseAttributes(match.Groups["attributes"].Captures.Select(x => x.Value));

                var tag = new Tag
                {
                    Attributes = attributes,
                    IsClosing = match.Groups["close"].Success,
                    IsSelfClosing = match.Groups["selfClose"].Success,
                    LineIndex = i,
                    Name = match.Groups["name"].Value,
                    Prefix = match.Groups["prefix"].Value,
                    Suffix = match.Groups["suffix"].Value
                };

                tags.Add(tag);
            }

            var elements = new List<Element<TData>>();

            for (var i = 0; i < tags.Count; i++)
            {
                var tag = tags[i];
                var isLastTag = i == (tags.Count - 1);

                var closingLineIndex = 0;

                if (tag.IsSelfClosing)
                {
                    closingLineIndex = tag.LineIndex;
                }
                else if (tag.IsOpening)
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

                if (!_dataSerializer.TryDeserialize(tag.Attributes, logger, out var data))
                {
                    // todo
                }

                var lineCount = Math.Max(0, closingLineIndex - tag.LineIndex) + 1;

                elements.Add(new Element<TData>
                {
                    Data = data,
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

        public void WriteElement(Element<TData> element, IReadOnlyCollection<string> content, List<string> lines)
        {
            var index = element.ElementLineIndex;

            lines.RemoveRange(index, element.ElementLineCount);

            lines.Insert(index++, GenerateOpeningTag(element));
            lines.InsertRange(index, content);
            lines.Insert(index + content.Count, GenerateClosingTag(element));
        }

        private string GenerateOpeningTag(Element<TData> element)
        {
            var attributes = _dataSerializer.Serialize(element.Data); // todo
            return $"{element.Prefix}<handyman-docs:{element.Name}{attributes}>{element.Suffix}";
        }

        private string GenerateClosingTag(Element<TData> element)
        {
            return $"{element.Prefix}</handyman-docs:{element.Name}>{element.Suffix}";
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

            public bool IsOpening => !IsClosing && !IsSelfClosing;
        }
    }
}