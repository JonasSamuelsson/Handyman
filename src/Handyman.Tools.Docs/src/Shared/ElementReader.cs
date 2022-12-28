using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Handyman.Tools.Docs.Shared;

public class ElementReader : IElementReader
{
    private const string TagPattern = "^(?<prefix>.*?)<(?<close>/)?handyman-docs:(?<name>[a-z0-9-._]+)( +(?<attributes>[a-z]+=\"[^\"]*\"))* *(?<selfClose>/)?>(?<postfix>.*)$";

    public IReadOnlyList<Element> ReadElements(string name, IReadOnlyList<string> lines)
    {
        return ReadElements(lines)
            .Where(x => x.Name == name)
            .Select(x => new Element
            {
                Content = x.Content,
                LineCount = x.LineCount,
                LineIndex = x.LineIndex,
                Name = x.Name,
                Postfix = x.Postfix,
                Prefix = x.Prefix,
                Attributes = x.Attributes
            })
            .ToList();
    }

    /// <remarks>public for testability</remarks>
    public static IReadOnlyList<Element> ReadElements(IReadOnlyList<string> lines)
    {
        var regex = new Regex(TagPattern, RegexOptions.IgnoreCase);

        var tags = new List<Tag>();

        var isInsideFencedCodeBlock = false;

        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];

            if (line.TrimStart().StartsWith("```"))
            {
                isInsideFencedCodeBlock = !isInsideFencedCodeBlock;
            }

            if (isInsideFencedCodeBlock)
                continue;

            var match = regex.Match(line);

            if (!match.Success)
                continue;

            var tag = new Tag
            {
                Attributes = new Attributes(),
                IsClosing = match.Groups["close"].Success,
                IsSelfClosing = match.Groups["selfClose"].Success,
                LineIndex = i,
                Name = match.Groups["name"].Value,
                Prefix = match.Groups["prefix"].Value,
                Postfix = match.Groups["postfix"].Value
            };

            var attributes = ParseAttributes(match.Groups["attributes"].Captures.Select(x => x.Value));

            foreach (var keyValuePair in attributes)
            {
                if (tag.Attributes.Contains(keyValuePair.Key))
                {
                    throw new Exception("todo");
                }

                tag.Attributes.Add(keyValuePair.Key, keyValuePair.Value);
            }

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
            else if (tag.IsOpening)
            {
                if (isLastTag)
                {
                    throw new Exception($"{tag.Name} tag on line {tag.LineNumber} does not have a matching close tag.");
                }

                var nextTag = tags[i + 1];

                if (tag.Name != nextTag.Name || !nextTag.IsClosing)
                {
                    throw new Exception($"{tag.Name} tag on line {tag.LineNumber} does not have a matching close tag.");
                }

                closingLineIndex = nextTag.LineIndex;
                i++;
            }
            else
            {
                throw new Exception($"found Unexpected {tag.Name} close tag on line {tag.LineNumber}.");
            }

            var elementLineCount = Math.Max(0, closingLineIndex - tag.LineIndex) + 1;

            var element = new Element
            {
                Attributes = tag.Attributes,
                Content = elementLineCount >= 2
                    ? lines.Skip(tag.LineIndex + 1).Take(elementLineCount - 2).ToList()
                    : new List<string>(),
                LineCount = elementLineCount,
                LineIndex = tag.LineIndex,
                Name = tag.Name,
                Postfix = tag.Postfix,
                Prefix = tag.Prefix
            };

            elements.Add(element);
        }

        return elements;
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
        public Attributes Attributes { get; set; }
        public bool IsClosing { get; set; }
        public bool IsSelfClosing { get; set; }
        public int LineIndex { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public string Postfix { get; set; }

        public bool IsOpening => !IsClosing && !IsSelfClosing;
        public int LineNumber => LineIndex + 1;
    }
}