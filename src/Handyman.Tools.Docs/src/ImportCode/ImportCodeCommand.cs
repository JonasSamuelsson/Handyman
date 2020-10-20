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
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly IElementSerializer<CodeBlockData> _codeElementSerializer;
        private readonly IElementSerializer<CodeBlockSourceData> _sourceElementSerializer;

        public ImportCodeCommand(ILogger logger, IFileSystem fileSystem, IElementSerializer<CodeBlockData> codeElementSerializer, IElementSerializer<CodeBlockSourceData> sourceElementSerializer)
        {
            _logger = logger;
            _fileSystem = fileSystem;
            _codeElementSerializer = codeElementSerializer;
            _sourceElementSerializer = sourceElementSerializer;
        }

        [Argument(0)]
        public string Target { get; set; }

        public void OnExecute()
        {
            var files = GetFiles();

            foreach (var file in files)
            {
                var lines = _fileSystem.File.ReadLines(file).ToList();

                var elements = _codeElementSerializer.TryDeserializeElements("code-block", lines, _logger);

                foreach (var element in elements.Reverse())
                {
                    var data = element.Data;

                    var sourcePath = GetSourcePath(data.Source, file);

                    if (!_fileSystem.File.Exists(sourcePath))
                    {
                        // todo
                        continue;
                    }

                    var contentLines = _fileSystem.File.ReadLines(sourcePath).ToList();

                    if (data.Id != null)
                    {
                        var sourceElements = _sourceElementSerializer.TryDeserializeElements("code-block-source", contentLines, _logger);

                        var sourceElement = sourceElements.FirstOrDefault(x => x.Data.Id == data.Id);

                        if (sourceElement == null)
                        {
                            // todo log
                            continue;
                        }

                        if (sourceElement.ContentLines == null)
                        {
                            // todo log
                            continue;
                        }

                        if (!sourceElement.ContentLines.IsInRange(contentLines, _logger))
                        {
                            continue;
                        }

                        contentLines = sourceElement.ContentLines.Apply(contentLines);
                    }
                    else if (data.Lines != null)
                    {
                        if (!data.Lines.IsInRange(contentLines, _logger))
                            continue;

                        contentLines = data.Lines.Apply(contentLines);
                    }

                    TrimIndentation(contentLines);

                    var language = data.Language ?? GetDefaultLanguage(sourcePath);
                    AddSyntaxHighlighting(contentLines, language);

                    _codeElementSerializer.WriteElement(element, contentLines, lines);
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

        private string GetDefaultLanguage(string path)
        {
            var extension = _fileSystem.Path.GetExtension(path);
            return ExtensionToLanguageLookup.GetLanguage(extension);
        }

        public static void AddSyntaxHighlighting(List<string> lines, string language)
        {
            lines.Insert(0, $"```{language}");
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