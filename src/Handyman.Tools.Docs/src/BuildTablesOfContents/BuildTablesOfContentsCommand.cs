using Handyman.Tools.Docs.Shared;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace Handyman.Tools.Docs.BuildTablesOfContents
{
    public class BuildTablesOfContentsCommand : Command<BuildTablesOfContentsCommand.Input>
    {
        public class Input : CommandSettings
        {
            [CommandArgument(0, "<target-path>")] public string TargetPath { get; set; }
        }

        private readonly IFileSystem _fileSystem;
        private readonly IElementReader _elementReader;
        private readonly IAttributesConverter _attributesConverter;

        public BuildTablesOfContentsCommand(IFileSystem fileSystem, IElementReader elementReader, IAttributesConverter attributesConverter)
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

            var patches = _elementReader.ReadElements("table-of-content", lines)
                .Select(element => new PatchEngine.Patch
                {
                    Content = GenerateTableOfContent(element, lines, markdownFilePath),
                    Element = element
                })
                .ToList();

            var newLines = PatchEngine.ApplyPatches(lines, patches);

            _fileSystem.File.WriteAllLines(markdownFilePath, newLines);
        }

        private IReadOnlyList<string> GenerateTableOfContent(Element element, IReadOnlyList<string> lines, string fileFullPath)
        {
            var attributes = _attributesConverter.ConvertTo<TableOfContentsAttributes>(element.Attributes);

            if (attributes.SourcePath == null)
            {
                return attributes.Levels.Current
                    ? GenerateTableOfContentForCurrentLevel(lines, element, attributes)
                    : GenerateTableOfContentForExplicitLevels(lines, attributes);
            }

            string sourceFullPath;

            if (_fileSystem.Path.IsPathRooted(attributes.SourcePath))
            {
                sourceFullPath = attributes.SourcePath;
            }
            else
            {
                var parentDirectoryPath = _fileSystem.Path.GetDirectoryName(fileFullPath);
                var combinedPaths = _fileSystem.Path.Combine(parentDirectoryPath, attributes.SourcePath);
                sourceFullPath = _fileSystem.Path.GetFullPath(combinedPaths);
            }

            var sourceLines = _fileSystem.File.ReadAllLines(sourceFullPath);

            return attributes.Levels.Current
                ? GenerateTableOfContentForCurrentLevel(sourceLines, element, attributes)
                : GenerateTableOfContentForExplicitLevels(sourceLines, attributes);
        }

        private static IReadOnlyList<string> GenerateTableOfContentForCurrentLevel(IReadOnlyList<string> lines, Element element, TableOfContentsAttributes attributes)
        {
            var headings = lines.ToMarkdownDocument()
                .Descendants<HeadingBlock>()
                .ToList();

            var candidates = headings
                .SkipWhile(x => x.Line < element.LineIndex)
                .ToList();

            if (!candidates.Any())
            {
                return ArraySegment<string>.Empty;
            }

            var currentLevel = candidates[0].Level;
            var levels = Enumerable.Range(currentLevel, attributes.Levels.CurrentAdditionalLevels + 1)
                .ToList();

            var inPlay = candidates
                .TakeWhile(x => x.Level >= currentLevel)
                .Where(x => levels.Contains(x.Level))
                .ToList();

            return GenerateTableOfContent(inPlay, attributes.ListType);
        }

        /// <remarks>public for testability</remarks>
        public static IReadOnlyList<string> GenerateTableOfContentForExplicitLevels(IReadOnlyList<string> lines, TableOfContentsAttributes attributes)
        {
            var headings = lines.ToMarkdownDocument()
                .Descendants<HeadingBlock>()
                .Where(x => attributes.Levels.ExplicitLevels.Contains(x.Level))
                .ToList();

            return GenerateTableOfContent(headings, attributes.ListType);
        }

        private static IReadOnlyList<string> GenerateTableOfContent(IEnumerable<HeadingBlock> headings, ListType listType)
        {
            var result = new List<string>();

            foreach (var heading in headings)
            {
                var indentation = new string(' ', (heading.Level - 1) * 2);
                var type = listType switch
                {
                    ListType.Ordered => "0.",
                    ListType.Unordered => "-",
                    _ => throw new TodoException()
                };
                var text = heading.Inline.ToMarkdownString();
                var link = heading.TryGetAttributes()!.Id;

                result.Add($"{indentation}{type} [{text}](#{link})");
            }

            result.UnIndentLines();

            return result;
        }
    }
}