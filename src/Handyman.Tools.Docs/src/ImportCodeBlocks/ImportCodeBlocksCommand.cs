using Handyman.Tools.Docs.Shared;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace Handyman.Tools.Docs.ImportCodeBlocks
{
    public class ImportCodeBlocksCommand : Command<ImportCodeBlocksCommand.Input>
    {
        public class Input : CommandSettings
        {
            [CommandArgument(0, "<target-path>")] public string TargetPath { get; set; }
        }

        private readonly IFileSystem _fileSystem;
        private readonly ILogger _logger;
        private readonly IElementReader _elementReader;
        private readonly IAttributesConverter _attributesConverter;

        public ImportCodeBlocksCommand(IFileSystem fileSystem, ILogger logger, IElementReader elementReader, IAttributesConverter attributesConverter)
        {
            _fileSystem = fileSystem;
            _logger = logger;
            _elementReader = elementReader;
            _attributesConverter = attributesConverter;
        }

        public override int Execute(CommandContext context, Input settings)
        {
            return Execute(settings);
        }

        /// <remarks>public for testability</remarks>
        public int Execute(Input settings)
        {
            var markdownFilePaths = _fileSystem.GetMarkdownFilePaths(settings.TargetPath);

            foreach (var markdownFilePath in markdownFilePaths)
            {
                ProcessMarkdownFile(markdownFilePath);
            }

            return 0;
        }

        private void ProcessMarkdownFile(string markdownFilePath)
        {
            using var scope = _logger.Scope(markdownFilePath);

            var lines = _fileSystem.File.ReadAllLines(markdownFilePath);

            var patches = _elementReader.ReadElements("code-block", lines)
                .Select(element => new PatchEngine.Patch
                {
                    Content = GenerateCodeBlock(element, markdownFilePath),
                    Element = element
                })
                .ToList();

            var newLines = PatchEngine.ApplyPatches(lines, patches);

            _fileSystem.File.WriteAllLines(markdownFilePath, newLines);
        }

        private IReadOnlyList<string> GenerateCodeBlock(Element element, string fileFullPath)
        {
            var attributes = _attributesConverter.ConvertTo<CodeBlockAttributes>(element.Attributes);

            string sourceFullPath;

            if (_fileSystem.Path.IsPathRooted(attributes.Source))
            {
                sourceFullPath = attributes.Source;
            }
            else
            {
                var parentDirectoryPath = _fileSystem.Path.GetDirectoryName(fileFullPath);
                var combinedPaths = _fileSystem.Path.Combine(parentDirectoryPath, attributes.Source);
                sourceFullPath = _fileSystem.Path.GetFullPath(combinedPaths);
            }

            var sourceExtension = _fileSystem.Path.GetExtension(sourceFullPath);
            var sourceLines = _fileSystem.File.ReadAllLines(sourceFullPath);

            return GenerateCodeBlock(sourceLines, attributes, sourceExtension);
        }

        /// <remarks>public for testability</remarks>
        public IReadOnlyList<string> GenerateCodeBlock(IReadOnlyList<string> sourceLines, CodeBlockAttributes attributes, string sourceExtension)
        {
            var syntaxHighlightingLanguage = attributes.Language ?? SyntaxHighlightingLanguageLookup.GetSyntaxHighlightingLanguage(sourceExtension);

            if (attributes.Id != null)
            {
                var elements = _elementReader.ReadElements("code-block-source", sourceLines);

                foreach (var element in elements)
                {
                    var sourceAttributes = _attributesConverter.ConvertTo<CodeBlockSourceAttributes>(element.Attributes);

                    if (sourceAttributes.Id != attributes.Id)
                        continue;

                    if (sourceAttributes.LinesSpec != null)
                    {
                        return GenerateCodeBlock(sourceLines, sourceAttributes.LinesSpec, syntaxHighlightingLanguage);
                    }

                    return GenerateCodeBlock(sourceLines,
                        LinesSpec.CreateForSection(element.ContentLineNumber, element.ContentLineCount),
                        syntaxHighlightingLanguage);
                }

                throw new Exception("todo");
            }

            if (attributes.LinesSpec != null)
            {
                return GenerateCodeBlock(sourceLines, attributes.LinesSpec, syntaxHighlightingLanguage);
            }

            return GenerateCodeBlock(sourceLines,
                LinesSpec.CreateForAllOf(sourceLines),
                syntaxHighlightingLanguage);
        }

        private IReadOnlyList<string> GenerateCodeBlock(IReadOnlyList<string> sourceLines, LinesSpec linesSpec, string language)
        {
            if (sourceLines.Count < linesSpec.Sections.Max(x => x.ToNumber))
            {
                throw new Exception("todo - lines out of range");
            }

            var result = linesSpec.Sections
                .SelectMany(section => sourceLines
                    .Skip(section.FromIndex)
                    .Take(section.Count))
                .ToList();

            result.UnIndentLines();

            result.Insert(0, $"```{language}");
            result.Add("```");

            return result;
        }
    }
}