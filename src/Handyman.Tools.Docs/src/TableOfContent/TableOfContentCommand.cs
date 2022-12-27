using Handyman.Tools.Docs.Utils;
using Markdig.Renderers.Html;
using Markdig.Renderers.Normalize;
using Markdig.Syntax;
using McMaster.Extensions.CommandLineUtils;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace Handyman.Tools.Docs.TableOfContent
{
    [Command("table-of-content")]
    public class TableOfContentCommand : Command<TableOfContentCommand.Input>
    {
        public class Input : CommandSettings
        {
            [CommandArgument(0, "<target-path>")] public string TargetPath { get; set; }
        }

        private readonly IFileSystem _fileSystem;
        private readonly IElementReader _elementReader;
        private readonly IAttributesConverter _attributesConverter;

        public TableOfContentCommand(IFileSystem fileSystem, IElementReader elementReader, IAttributesConverter attributesConverter)
        {
            _fileSystem = fileSystem;
            _elementReader = elementReader;
            _attributesConverter = attributesConverter;
        }

        public override int Execute(CommandContext context, Input settings)
        {
            var markdownFilePaths = GetMarkdownFilePaths(settings.TargetPath);

            foreach (var markdownFilePath in markdownFilePaths)
            {
                ProcessFile(markdownFilePath);
            }

            return 0;
        }

        private IReadOnlyList<string> GetMarkdownFilePaths(string targetPath)
        {
            if (_fileSystem.File.Exists(targetPath))
            {
                return new[] { targetPath };
            }

            if (_fileSystem.Directory.Exists(targetPath))
            {
                return _fileSystem.Directory.GetFiles(targetPath, "*.md", SearchOption.AllDirectories);
            }

            throw new Exception("todo");
        }

        private void ProcessFile(string markdownFilePath)
        {
            var lines = _fileSystem.File.ReadAllLines(markdownFilePath);

            var patches = _elementReader.ReadElements("table-of-content", lines)
                .Select(element => GeneratePatch(element, lines, markdownFilePath))
                .ToList();

            var newLines = PatchEngine.ApplyPatches(lines, patches);

            _fileSystem.File.WriteAllLines(markdownFilePath, newLines);
        }

        private PatchEngine.Patch GeneratePatch(E element, IReadOnlyList<string> lines, string fileFullPath)
        {
            var attributes = _attributesConverter.To<Attributes>(element.Attributes);

            IReadOnlyList<string> tableOfContent;

            if (attributes.SourcePath == null)
            {
                tableOfContent = GenerateTableOfContent(lines, attributes);
            }
            else
            {
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

                tableOfContent = GenerateTableOfContent(sourceLines, attributes);
            }

            return new PatchEngine.Patch
            {
                Content = tableOfContent,
                Element = element
            };
        }

        /// <remarks>public for testability</remarks>
        public static IReadOnlyList<string> GenerateTableOfContent(IReadOnlyList<string> lines, Attributes attributes)
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

    public class Attributes
    {
        public IReadOnlyCollection<int> Levels { get; set; } = new[] { 1, 2, 3, 4, 5, 6 };
        public ListType ListType { get; set; } = ListType.Unordered;
        public string SourcePath { get; set; }
    }

    public enum ListType
    {
        Unordered = 0,
        Ordered = 1,
    }
}