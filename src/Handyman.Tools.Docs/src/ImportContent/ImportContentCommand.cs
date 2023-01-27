using Handyman.Tools.Docs.Shared;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;

namespace Handyman.Tools.Docs.ImportContent;

public class ImportContentCommand : Command<ImportContentCommand.Input>
{
    public class Input : CommandSettings
    {
        [CommandArgument(0, "<target-path>")] public string TargetPath { get; set; }
    }

    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;
    private readonly IElementReader _elementReader;
    private readonly IAttributesConverter _attributesConverter;

    public ImportContentCommand(IFileSystem fileSystem, ILogger logger, IElementReader elementReader, IAttributesConverter attributesConverter)
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

        var patches = _elementReader.ReadElements("content", lines)
            .Select(element => new PatchEngine.Patch
            {
                Content = GenerateContent(element, markdownFilePath),
                Element = element
            })
            .ToList();

        var newLines = PatchEngine.ApplyPatches(lines, patches);

        _fileSystem.File.WriteAllLines(markdownFilePath, newLines);
    }

    private IReadOnlyList<string> GenerateContent(Element element, string fileFullPath)
    {
        var attributes = _attributesConverter.ConvertTo<ContentAttributes>(element.Attributes);

        string sourceFullPath;

        if (_fileSystem.Path.IsPathRooted(attributes.Source))
        {
            sourceFullPath = attributes.Source;
        }
        else
        {
            sourceFullPath = _fileSystem.ConstructPathRelativeToFile(fileFullPath, attributes.Source);
        }

        var sourceLines = _fileSystem.File.ReadAllLines(sourceFullPath);

        return GenerateContent(sourceLines, attributes);
    }

    /// <remarks>public for testability</remarks>
    public IReadOnlyList<string> GenerateContent(IReadOnlyList<string> sourceLines, ContentAttributes attributes)
    {
        if (attributes.Id != null)
        {
            var elements = _elementReader.ReadElements("content-source", sourceLines);

            foreach (var element in elements)
            {
                var sourceAttributes = _attributesConverter.ConvertTo<ContentSourceAttributes>(element.Attributes);

                if (sourceAttributes.Id != attributes.Id)
                    continue;

                if (sourceAttributes.LinesSpec != null)
                {
                    return GenerateContent(sourceLines, sourceAttributes.LinesSpec);
                }

                return GenerateContent(sourceLines, LinesSpec.CreateForSection(element.ContentLineNumber, element.ContentLineCount));
            }

            throw new TodoException();
        }

        if (attributes.LinesSpec != null)
        {
            return GenerateContent(sourceLines, attributes.LinesSpec);
        }

        return GenerateContent(sourceLines, LinesSpec.CreateForAllOf(sourceLines));
    }

    private IReadOnlyList<string> GenerateContent(IReadOnlyList<string> sourceLines, LinesSpec linesSpec)
    {
        if (sourceLines.Count < linesSpec.Sections.Max(x => x.ToNumber))
        {
            throw new Exception("todo - lines out of range");
        }

        return linesSpec.Sections
            .SelectMany(section => sourceLines
                .Skip(section.FromIndex)
                .Take(section.Count))
            .ToList();
    }
}