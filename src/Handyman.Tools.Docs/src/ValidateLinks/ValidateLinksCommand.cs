using Handyman.Tools.Docs.Shared;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Spectre.Console.Cli;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Handyman.Tools.Docs.ValidateLinks;

public class ValidateLinksCommand : AsyncCommand<ValidateLinksCommand.Input>
{
    public class Input : CommandSettings
    {
        [CommandArgument(0, "<target-path>")] public string TargetPath { get; set; }

        [CommandOption("--exit-code")] public bool ExitCode { get; set; }
        [CommandOption("--link-type")] public LinkType LinkType { get; set; } = LinkType.Local;
        [CommandOption("--verbosity")] public Verbosity Verbosity { get; set; } = Verbosity.Normal;
    }

    private readonly IFileSystem _fileSystem;
    private readonly ILogger _logger;
    private readonly IHttpClient _httpClient;

    public ValidateLinksCommand(IFileSystem fileSystem, ILogger logger, IHttpClient httpClient)
    {
        _fileSystem = fileSystem;
        _logger = logger;
        _httpClient = httpClient;
    }

    public override Task<int> ExecuteAsync(CommandContext context, Input settings)
    {
        return Execute(settings);
    }

    /// <remarks>public for testability</remarks>
    public async Task<int> Execute(Input settings)
    {
        _logger.Verbosity = settings.Verbosity;

        var markdownFilePaths = _fileSystem.GetMarkdownFilePaths(settings.TargetPath);
        var result = true;

        foreach (var markdownFilePath in markdownFilePaths)
        {
            result &= await ValidateFile(markdownFilePath, settings);
        }

        return (!settings.ExitCode || result) ? 0 : 1;
    }

    private async Task<bool> ValidateFile(string markdownFilePath, Input settings)
    {
        using var _ = _logger.Scope(markdownFilePath);

        var lines = _fileSystem.File.ReadAllLines(markdownFilePath);
        var document = lines.ToMarkdownDocument();
        var links = document.Descendants<LinkInline>();

        var result = true;

        foreach (var link in links)
        {
            result &= await ValidateLink(link, markdownFilePath, settings);
        }

        return result;
    }

    private async Task<bool> ValidateLink(LinkInline link, string markdownFilePath, Input settings)
    {
        using var __ = _logger.Scope($"{link.ToMarkdownString()}; line {link.Line + 1}, column {link.Column}");

        var url = link.Url;

        if (string.IsNullOrWhiteSpace(url) || url == "#")
        {
            _logger.WriteDebug("Skipping empty link.");
            return true;
        }

        if (url.StartsWith("http://") || url.StartsWith("https://"))
        {
            if (!settings.LinkType.HasFlag(LinkType.Remote))
            {
                _logger.WriteDebug("Skipping remote link.");
                return true;
            }

            return await ValidateRemoteLink(url);
        }

        if (!settings.LinkType.HasFlag(LinkType.Local))
        {
            _logger.WriteDebug("Skipping local link.");
            return true;
        }

        return ValidateLocalLink(url, markdownFilePath);
    }

    private async Task<bool> ValidateRemoteLink(string url)
    {
        try
        {
            var response = await _httpClient.Get(url);

            if (!response.IsSuccessStatusCode)
            {
                _logger.WriteError($"{response.StatusCode} {response.ReasonPhrase}");
                return false;
            }

            _logger.WriteDebug("Ok");
            return true;
        }
        catch (Exception exception)
        {
            _logger.WriteError(exception.Message);
            return false;
        }
    }

    private bool ValidateLocalLink(string url, string markdownFilePath)
    {
        var anchor = "";
        var hasAnchor = url.Contains('#');

        if (hasAnchor)
        {
            var index = url.IndexOf('#');

            if (index + 1 == url.Length)
            {
                hasAnchor = false;
            }
            else
            {
                anchor = url.Substring(index + 1);
            }

            url = index == 0 ? markdownFilePath : url.Substring(0, index);
        }

        if (!_fileSystem.Path.IsPathFullyQualified(url))
        {
            var relativePath = url;

            try
            {
                url = _fileSystem.ConstructPathRelativeToFile(markdownFilePath, relativePath);

                _logger.WriteDebug($"{relativePath} >> {url}");
            }
            catch (AppException exception)
            {
                _logger.WriteDebug($"{relativePath} >> {exception.Message}");
                return false;
            }
        }

        if (!_fileSystem.File.Exists(url))
        {
            _logger.WriteError("Not found");
            return false;
        }

        if (!hasAnchor || !url.EndsWith(".md"))
        {
            _logger.WriteDebug("Ok");
            return true;
        }

        var targetDocument = _fileSystem.File.ReadAllLines(url).ToMarkdownDocument();

        foreach (var heading in targetDocument.Descendants<HeadingBlock>())
        {
            var id = heading.TryGetAttributes()?.Id;
            if (id != anchor) continue;
            _logger.WriteDebug("Ok");
            return true;
        }

        _logger.WriteError("Anchor not found");
        return false;
    }

    private string GetAbsoluteFilePath(string relativePath, string currentFilePath)
    {
        var url = _fileSystem.ConstructPathRelativeToFile(currentFilePath, relativePath);

        _logger.WriteDebug($"{relativePath} >> {url}");

        return url;
    }
}