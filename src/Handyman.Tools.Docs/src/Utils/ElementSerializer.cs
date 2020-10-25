using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Handyman.Tools.Docs.Utils
{
    public class ElementSerializer<TData> : IElementSerializer<TData>
        where TData : ElementData
    {
        private const string Pattern = "^(?<prefix>.*?)<(?<close>/)?handyman-docs:(?<name>[a-z0-9-._]+)( +(?<attributes>[a-z]+=\"[^\"]*\"))* *(?<selfClose>/)?>(?<suffix>.*)$";

        private readonly IDataSerializer<TData> _dataSerializer;

        public ElementSerializer(IDataSerializer<TData> dataSerializer)
        {
            _dataSerializer = dataSerializer;
        }

        public bool TryDeserializeElements(string elementName, IReadOnlyList<string> lines, ILogger logger, out IReadOnlyList<Element<TData>> elements)
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

            elements = null;

            var list = new List<Element<TData>>();

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
                    {
                        logger.WriteError(
                            $"{tag.Name} tag on line {tag.LineNumber} does not have a matching close tag.");
                        return false;
                    }

                    var nextTag = tags[i + 1];

                    if (tag.Name != nextTag.Name || !nextTag.IsClosing)
                    {
                        logger.WriteError(
                            $"{tag.Name} tag on line {tag.LineNumber} does not have a matching close tag.");
                        return false;
                    }

                    closingLineIndex = nextTag.LineIndex;
                    i++;
                }
                else
                {
                    {
                        logger.WriteError($"found Unexpected {tag.Name} close tag on line {tag.LineNumber}.");
                        return false;
                    }
                }

                if (elementName != null && tag.Name != elementName)
                    continue;

                using (logger.CreateScope($"Parsing {tag.Name} on line {tag.LineNumber}"))
                {
                    if (!_dataSerializer.TryDeserialize(tag.Attributes, logger, out var data))
                        return false;

                    var lineCount = Math.Max(0, closingLineIndex - tag.LineIndex) + 1;

                    list.Add(new Element<TData>
                    {
                        Data = data,
                        ContentLines = lineCount <= 2
                            ? null
                            : new Lines
                            {
                                Count = lineCount - 2,
                                FromNumber = tag.LineIndex + 2,
                            },
                        ElementLines = new Lines
                        {
                            Count = lineCount,
                            FromNumber = tag.LineIndex + 1
                        },
                        Name = tag.Name,
                        Prefix = tag.Prefix,
                        Suffix = tag.Suffix
                    });
                }
            }

            elements = list;
            return true;
        }

        public void WriteElement(Element<TData> element, IReadOnlyCollection<string> content, List<string> lines)
        {
            var index = element.ElementLines.FromIndex;

            lines.RemoveRange(index, element.ElementLines.Count);

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
            public int LineNumber => LineIndex + 1;
        }
    }
}