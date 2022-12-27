using Handyman.Tools.Docs.Shared;
using Markdig.Renderers.Html;
using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

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
                return GenerateTableOfContent(lines, attributes);
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

            return GenerateTableOfContent(sourceLines, attributes);
        }

        /// <remarks>public for testability</remarks>
        public static IReadOnlyList<string> GenerateTableOfContent(IReadOnlyList<string> lines, TableOfContentsAttributes attributes)
        {
            var headings = lines.ToMarkdownDocument().Descendants<HeadingBlock>().ToList();

            var result = new List<string>();

            foreach (var heading in headings)
            {
                if (!attributes.Levels.Contains(heading.Level))
                    continue;

                var indentation = new string(' ', (heading.Level - 1) * 2);
                var type = attributes.ListType switch
                {
                    ListType.Ordered => "0.",
                    ListType.Unordered => "-",
                    _ => throw new Exception("todo")
                };
                var text = GetText(heading.Inline);
                var link = heading.TryGetAttributes()!.Id;

                result.Add($"{indentation}{type} [{text}](#{link})");
            }

            result.UnIndentLines();

            return result;
        }

        private static string GetText(MarkdownObject markdownObject)
        {
            var stringBuilder = new StringBuilder();
            new NormalizeRenderer(new StringWriter(stringBuilder)).Render(markdownObject);
            return stringBuilder.ToString();
        }
    }
}