using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Handyman.Tools.Docs.Utils
{
    //    public interface IFileSys
    //    {
    //        string GetFullPath(string path);

    //        bool DirectoryExists(string path);
    //        bool FileExists(string path);
    //        IReadOnlyList<string> ListFiles(string path, string pattern);

    //        IReadOnlyList<string> ReadFile(string path);
    //        void WriteFile(string path, IEnumerable<string> lines);

    //        string GetParentDirectoryPath(string path)
    //        {
    //            return Path.GetDirectoryName(path);
    //        }

    //        string CombinePaths(string path, string relativePath)
    //        {
    //            return Path.Combine(path, relativePath);
    //        }
    //    }

    //    public class FileSys : IFileSys
    //    {
    //        public string GetFullPath(string path)
    //        {
    //            return Path.GetFullPath(path);
    //        }

    //        public bool DirectoryExists(string path)
    //        {
    //            return Directory.Exists(path);
    //        }

    //        public bool FileExists(string path)
    //        {
    //            return File.Exists(path);
    //        }

    //        public IReadOnlyList<string> ListFiles(string path, string pattern)
    //        {
    //            return Directory.GetFiles(path, pattern, SearchOption.AllDirectories);
    //        }

    //        public IReadOnlyList<string> ReadFile(string path)
    //        {
    //            return File.ReadAllLines(path);
    //        }

    //        public void WriteFile(string path, IEnumerable<string> lines)
    //        {
    //            File.WriteAllLines(path, lines);
    //        }
    //    }

    public class E
    {
        public IReadOnlyList<string> Content { get; set; }
        public int LineIndex { get; set; }
        public int LineCount { get; set; }
        public string Name { get; set; }
        public string Prefix { get; set; }
        public Attributes Attributes { get; set; }
        public string Postfix { get; set; }
    }

    //public class E<TProperties> : E
    //{
    //    public TProperties Properties { get; set; }
    //}

    //public interface IElementReader<TElementProperties>
    //{
    //    IReadOnlyList<E<TElementProperties>> ReadElements(string name, IReadOnlyList<string> lines);
    //}

    //public class ElementReader<TElementProperties> : IElementReader<TElementProperties>
    //{
    //    private readonly IElementReader _elementReader;
    //    private readonly IObjectParser<TElementProperties> _objectParser;

    //    public ElementReader(IElementReader elementReader, IObjectParser<TElementProperties> objectParser)
    //    {
    //        _elementReader = elementReader;
    //        _objectParser = objectParser;
    //    }

    //    public IReadOnlyList<E<TElementProperties>> ReadElements(string name, IReadOnlyList<string> lines)
    //    {
    //        return _elementReader.ReadElements(lines)
    //            .Where(x => x.Name == name)
    //            .Select(x => new E<TElementProperties>
    //            {
    //                Content = x.Content,
    //                LineCount = x.LineCount,
    //                LineIndex = x.LineIndex,
    //                Name = x.Name,
    //                Postfix = x.Postfix,
    //                Prefix = x.Prefix,
    //                Attributes = x.Attributes
    //            })
    //            .ToList();
    //    }
    //}

    public interface IElementReader
    {
        IReadOnlyList<E> ReadElements(string name, IReadOnlyList<string> lines);
    }

    public class ElementReader : IElementReader
    {
        private const string TagPattern = "^(?<prefix>.*?)<(?<close>/)?handyman-docs:(?<name>[a-z0-9-._]+)( +(?<attributes>[a-z]+=\"[^\"]*\"))* *(?<selfClose>/)?>(?<postfix>.*)$";

        public IReadOnlyList<E> ReadElements(string name, IReadOnlyList<string> lines)
        {
            return ReadElements(lines)
                .Where(x => x.Name == name)
                .Select(x => new E
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
        public static IReadOnlyList<E> ReadElements(IReadOnlyList<string> lines)
        {
            var regex = new Regex(TagPattern, RegexOptions.IgnoreCase);

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
                    Attributes = new Attributes(),
                    IsClosing = match.Groups["close"].Success,
                    IsSelfClosing = match.Groups["selfClose"].Success,
                    LineIndex = i,
                    Name = match.Groups["name"].Value,
                    Prefix = match.Groups["prefix"].Value,
                    Postfix = match.Groups["postfix"].Value
                };

                foreach (var keyValuePair in ParseAttributes(match.Groups["attributes"].Captures.Select(x => x.Value)))
                {
                    if (tag.Attributes.Contains(keyValuePair.Key))
                    {
                        throw new Exception("todo");
                    }

                    tag.Attributes.Add(keyValuePair.Key, keyValuePair.Value);
                }

                tags.Add(tag);
            }

            var elements = new List<E>();

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

                var element = new E
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

    public interface IAttributesConverter
    {
        public TAttributes To<TAttributes>(Attributes attributes) where TAttributes : new();
    }

    public class AttributesConverter : IAttributesConverter
    {
        private readonly IEnumerable<IValConverter> _valueConverters;

        public AttributesConverter(IEnumerable<IValConverter> valueConverters)
        {
            _valueConverters = valueConverters;
        }

        public TAttributes To<TAttributes>(Attributes attributes) where TAttributes : new()
        {
            var set = attributes.Select(x => x.Key).ToHashSet(StringComparer.OrdinalIgnoreCase);

            var tAttributes = new TAttributes();

            foreach (var property in tAttributes.GetType().GetProperties())
            {
                if (!attributes.TryGet(property.Name, out var stringValue))
                    continue;

                set.Remove(property.Name);

                if (property.PropertyType == typeof(string))
                {
                    property.SetValue(tAttributes, stringValue);
                    continue;
                }

                object value = null;
                if (_valueConverters.Any(x => x.TryConvert(stringValue, property.PropertyType, out value)))
                {
                    property.SetValue(tAttributes, value);
                    continue;
                }

                throw new Exception("todo");
            }

            if (set.Any())
            {
                throw new Exception($"todo - unhandled attributes; {string.Join(", ", set)}");
            }

            return tAttributes;
        }
    }

    public interface IValConverter
    {
        public bool TryConvert(string s, Type targetType, out object value);
    }

    public class EnumValueConverter : IValConverter
    {
        public bool TryConvert(string s, Type targetType, out object value)
        {
            if (!targetType.IsEnum)
            {
                value = null;
                return false;
            }

            value = Enum.Parse(targetType, s, ignoreCase: true);
            return true;
        }
    }

    public class Attributes : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Dictionary<string, string> _dictionary = new();

        public void Add(string name, string value)
        {
            _dictionary.Add(name, value);
        }

        public bool Contains(string name)
        {
            return TryGet(name, out _);
        }

        public string Get(string name)
        {
            return TryGet(name, out var value) ? value : throw new Exception("todo");
        }

        public bool TryGet(string name, out string value)
        {
            return _dictionary.TryGetValue(name, out value);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public static class PatchEngine
    {
        public static IReadOnlyList<string> ApplyPatches(IEnumerable<string> lines, IEnumerable<Patch> patches)
        {
            var result = lines.ToList();

            foreach (var patch in patches.Reverse())
            {
                var element = patch.Element;
                var index = element.LineIndex;
                var name = $"handyman-docs:{element.Name}";
                var attributes = string.Join(" ", element.Attributes.Select(x => $"{x.Key}=\"{x.Value}\""));
                var nameAndAttributes = $"{name} {attributes}".Trim();

                result.RemoveRange(index, element.LineCount);

                result.Insert(index++, $"{element.Prefix}<{nameAndAttributes}>{element.Postfix}");
                result.InsertRange(index, patch.Content);
                result.Insert(index + patch.Content.Count, $"{element.Prefix}</{name}>{element.Postfix}");
            }

            return result;
        }

        public class Patch
        {
            public E Element { get; set; }
            public IReadOnlyList<string> Content { get; set; }
        }
    }
}