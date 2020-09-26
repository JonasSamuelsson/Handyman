using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Handyman.Tools.Docs.Utils
{
    public class ElementsParser
    {
        private readonly IEnumerable<IAttributesParser> _attributesParsers;
        private const string Pattern = "^(?<prefix>.*?)<(?<close>/)?handyman-docs:(?<name>[a-z0-9-._]+)( +(?<attributes>[a-z]+=\"[^\"]*\"))* *(?<selfClose>/)?>(?<suffix>.*)$";

        public ElementsParser(IEnumerable<IAttributesParser> attributesParsers)
        {
            _attributesParsers = attributesParsers;
        }

        public IReadOnlyCollection<Element> Parse(string elementName, IReadOnlyList<string> lines)
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

            var elements = new List<Element>();

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

                _attributesParsers.SingleOrDefault(x => x.CanHandle(tag.Name))?.Validate(tag.Attributes);

                var lineCount = Math.Max(0, closingLineIndex - tag.LineIndex) + 1;

                elements.Add(new Element
                {
                    Attributes = tag.Attributes,
                    FromLineIndex = tag.LineIndex,
                    LineCount = lineCount,
                    Name = tag.Name
                });
            }

            return elements;
        }

        private static Attribute[] ParseAttributes(IEnumerable<string> strings)
        {
            return strings
                .Select(s =>
                {
                    var index = s.IndexOf("=", StringComparison.Ordinal);
                    return new Attribute
                    {
                        Name = s.Substring(0, index),
                        Value = s.Substring(index + 2, s.Length - (index + 3))
                    };
                })
                .ToArray();
        }

        private class Tag
        {
            public Attribute[] Attributes { get; set; }
            public bool IsClosing { get; set; }
            public bool IsSelfClosing { get; set; }
            public int LineIndex { get; set; }
            public string Name { get; set; }
            public string Prefix { get; set; }
            public string Suffix { get; set; }

            public bool IsOpen => !IsClosing && !IsSelfClosing;
        }
    }
}