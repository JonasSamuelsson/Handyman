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
        private readonly IElementReader _elementReader;
        private readonly IAttributesConverter _attributesConverter;

        public ImportCodeBlocksCommand(IFileSystem fileSystem, IElementReader elementReader, IAttributesConverter attributesConverter)
        {
            _fileSystem = fileSystem;
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
            var lines = _fileSystem.File.ReadAllLines(markdownFilePath);

            var patches = _elementReader.ReadElements("code-block", lines)
                .Select(element => new PatchEngine.Patch
                {
                    Content = GenerateCodeBlock(element, lines, markdownFilePath),
                    Element = element
                })
                .ToList();

            var newLines = PatchEngine.ApplyPatches(lines, patches);

            _fileSystem.File.WriteAllLines(markdownFilePath, newLines);
        }

        private IReadOnlyList<string> GenerateCodeBlock(Element element, IReadOnlyList<string> lines, string fileFullPath)
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

            var sourceLines = _fileSystem.File.ReadAllLines(sourceFullPath);

            return GenerateCodeBlock(sourceLines, attributes, sourceFullPath);
        }

        /// <remarks>public for testability</remarks>
        public IReadOnlyList<string> GenerateCodeBlock(IReadOnlyList<string> sourceLines, CodeBlockAttributes attributes, string sourceFilePath)
        {
            if (attributes.Id != null)
            {
                var elements = _elementReader.ReadElements("code-block-source", sourceLines);

                foreach (var element in elements)
                {
                    var sourceAttributes = _attributesConverter.ConvertTo<CodeBlockSourceAttributes>(element.Attributes);

                    if (sourceAttributes.Id != attributes.Id)
                        continue;

                    if (sourceAttributes.Lines != null)
                    {
                        return GenerateCodeBlock(sourceLines, sourceAttributes.Lines, sourceFilePath);
                    }

                    return GenerateCodeBlock(sourceLines,
                        new Lines
                        {
                            Count = element.LineCount - 2,
                            FromNumber = element.LineIndex + 2
                        },
                        sourceFilePath);
                }

                throw new Exception("todo");
            }

            if (attributes.Lines != null)
            {
                return GenerateCodeBlock(sourceLines, attributes.Lines, sourceFilePath);
            }

            return GenerateCodeBlock(sourceLines,
                new Lines
                {
                    Count = sourceLines.Count,
                    FromNumber = 1
                },
                sourceFilePath);
        }

        private IReadOnlyList<string> GenerateCodeBlock(IReadOnlyList<string> sourceLines, Lines lines, string sourceFilePath)
        {
            if (sourceLines.Count < (lines.FromIndex + lines.Count))
            {
                throw new Exception("todo - lines out of range");
            }

            var result = sourceLines
                .Skip(lines.FromIndex)
                .Take(lines.Count)
                .ToList();

            result.UnIndentLines();

            result.Insert(0, $"```{GetSyntaxHighlightingLanguage(sourceFilePath)}");
            result.Add("```");

            return result;
        }

        private string GetSyntaxHighlightingLanguage(string path)
        {
            var extension = _fileSystem.Path.GetExtension(path);
            return SyntaxHighlightingLanguageLookup.GetSyntaxHighlightingLanguage(extension);
        }
    }
}