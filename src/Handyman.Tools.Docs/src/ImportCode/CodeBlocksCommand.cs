using Handyman.Tools.Docs.Utils;
using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace Handyman.Tools.Docs.ImportCode
{
    [Command("code-blocks")]
    public class CodeBlocksCommand
    {
        private readonly ILogger _logger;
        private readonly IFileSystem _fileSystem;
        private readonly IElementSerializer<CodeBlockData> _codeElementSerializer;
        private readonly IElementSerializer<CodeBlockSourceData> _sourceElementSerializer;

        public CodeBlocksCommand(ILogger logger, IFileSystem fileSystem, IElementSerializer<CodeBlockData> codeElementSerializer, IElementSerializer<CodeBlockSourceData> sourceElementSerializer)
        {
            _logger = logger;
            _fileSystem = fileSystem;
            _codeElementSerializer = codeElementSerializer;
            _sourceElementSerializer = sourceElementSerializer;
        }

        [Argument(0)] public string Target { get; set; }

        public void OnExecute()
        {
            var markdownFiles = GetMarkdownFiles();

            foreach (var markdownFile in markdownFiles)
            {
                using (_logger.CreateScope($"File: {markdownFile}"))
                {
                    var lines = _fileSystem.File.ReadLines(markdownFile).ToList();

                    if (!_codeElementSerializer.TryDeserializeElements("code-block", lines, _logger, out var codeBlockElements))
                        continue;

                    foreach (var codeBlockElement in codeBlockElements.Reverse())
                    {
                        using (_logger.CreateScope($"code-block: line {codeBlockElement.ElementLines.FromNumber}"))
                        {
                            var data = codeBlockElement.Data;

                            var sourcePath = GetSourcePath(data.Source, markdownFile);

                            using (_logger.CreateScope($"Source: '{sourcePath}'"))
                            {
                                if (!_fileSystem.File.Exists(sourcePath))
                                {
                                    _logger.WriteError("File not found.");
                                    continue;
                                }

                                var contentLines = _fileSystem.File.ReadLines(sourcePath).ToList();

                                if (data.Lines != null)
                                {
                                    if (!data.Lines.TryTrim(contentLines))
                                    {
                                        var codeBlockElementLines = codeBlockElement.ElementLines.ToUserFriendlyString();
                                        _logger.WriteError($"code-block on {codeBlockElementLines} has invalid lines range.");
                                        continue;
                                    }
                                }
                                else if (data.Id != null)
                                {
                                    if (!_sourceElementSerializer.TryDeserializeElements("code-block-source", contentLines, _logger, out var sourceElements))
                                        continue;

                                    var sourceElement = sourceElements.FirstOrDefault(x => x.Data.Id == data.Id);

                                    if (sourceElement == null)
                                    {
                                        _logger.WriteError($"code-block-source id='{data.Id}' not found.");
                                        continue;
                                    }

                                    var sourceLines = sourceElement.Data.Lines ?? sourceElement.ContentLines;

                                    if (sourceLines == null)
                                    {
                                        _logger.WriteError($"code-block-source id='{data.Id}' has no content.");
                                        continue;
                                    }

                                    if (!sourceLines.TryTrim(contentLines))
                                    {
                                        _logger.WriteError($"code-block-source id='{data.Id}' has invalid lines range.");
                                        continue;
                                    }
                                }

                                TrimIndentation(contentLines);

                                var language = data.Language ?? GetDefaultLanguage(sourcePath);
                                AddSyntaxHighlighting(contentLines, language);

                                _codeElementSerializer.WriteElement(codeBlockElement, contentLines, lines);
                            }
                        }
                    }

                    _fileSystem.File.WriteAllLines(markdownFile, lines);
                }
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
                    .Take(2)
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

        private IEnumerable<string> GetMarkdownFiles()
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