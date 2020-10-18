using Handyman.Tools.Docs.Utils;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Handyman.Tools.Docs.ImportCode
{
    [Command("import-code")]
    public class ImportCodeCommand
    {
        private readonly IFileSystem _fileSystem;
        private readonly IElementSerializer<ImportCodeAttributes> _importCodeElementSerializer;

        public ImportCodeCommand(IFileSystem fileSystem, IElementSerializer<ImportCodeAttributes> importCodeElementSerializer)
        {
            _fileSystem = fileSystem;
            _importCodeElementSerializer = importCodeElementSerializer;
        }

        [Argument(0)]
        public string Target { get; set; }

        public void OnExecute()
        {
            var files = GetFiles();

            foreach (var file in files)
            {
                var lines = _fileSystem.File.ReadLines(file).ToList();

                var elements = _importCodeElementSerializer.TryDeserializeElements(lines, "import-code");

                foreach (var element in elements.Reverse())
                {
                    var attributes = element.Attributes;

                    var sourcePath = GetSourcePath(attributes.Source, file);

                    if (!_fileSystem.File.Exists(sourcePath))
                    {
                        // todo
                        continue;
                    }

                    var sourceLines = _fileSystem.File.ReadLines(sourcePath).ToList();

                    if (attributes.Id != null)
                    {
                        var refElements = _elementsParser.Parse("reference", sourceLines);

                        foreach (var refElement in refElements)
                        {
                            // todo reduce number of dependencies
                            // todo validate
                            // todo error handling
                            var attr = _refAttrDeserializer.Deserialize(refElement.Attributes);
                            if (attr.Id != attributes.Id) continue;
                            sourceLines = sourceLines
                                .Skip(refElement.ContentLineIndex ?? 0)
                                .Take(refElement.ContentLineCount)
                                .ToList();
                        }
                        // todo
                    }
                    else if (attributes.Lines != null)
                    {
                        // todo check line numbers
                        sourceLines = sourceLines
                            .Skip(attributes.Lines.FromLineIndex)
                            .Take(attributes.Lines.LineCount)
                            .ToList();
                    }

                    TrimIndentation(sourceLines);
                    AddSyntaxHighlighting(sourceLines, _fileSystem.Path.GetExtension(sourcePath));

                    _elementWriter.Write(lines, element, sourceLines);
                }

                _fileSystem.File.WriteAllLines(file, lines);
            }
        }

        public static void TrimIndentation(List<string> lines)
        {
            if (lines.Count == 0)
                return;

            for (var i = 0; i < lines.Count; i++)
            {
                var line = lines[i];

                if (!string.IsNullOrWhiteSpace(line))
                    continue;

                lines[i] = string.Empty;
            }

            while (true)
            {
                var firstCharacters = lines
                    .Where(x => x.Length != 0)
                    .Select(x => x[0])
                    .Distinct()
                    .ToArray();

                if (firstCharacters.Length != 1)
                    break;

                var character = firstCharacters[0];

                if (character != ' ' && character != '\t')
                    break;

                for (var i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];

                    if (line.Length == 0)
                        continue;

                    lines[i] = line.Substring(1);
                }
            }
        }

        public static void AddSyntaxHighlighting(List<string> lines, string extension)
        {
            lines.Insert(0, $"```{ExtensionToLanguageLookup.GetLanguage(extension)}");
            lines.Add("```");
        }

        private IEnumerable<string> GetFiles()
        {
            var target = Path.GetFullPath(Target ?? Environment.CurrentDirectory);

            if (_fileSystem.File.Exists(target))
            {
                return new[] { target };
            }

            if (_fileSystem.Directory.Exists(target))
            {
                return _fileSystem.Directory.GetFiles(target, "*.md", SearchOption.AllDirectories);
            }

            throw new NotImplementedException();
        }

        private string GetSourcePath(string source, string file)
        {
            if (_fileSystem.Path.IsPathFullyQualified(source))
            {
                return source;
            }

            var directory = _fileSystem.Path.GetDirectoryName(file);

            return _fileSystem.Path.Combine(directory, source);
        }
    }
}