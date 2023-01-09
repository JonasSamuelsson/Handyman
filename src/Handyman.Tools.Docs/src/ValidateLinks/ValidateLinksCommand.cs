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
            result &= await Validate(markdownFilePath, settings);
        }

        return (!settings.ExitCode || result) ? 0 : 1;
    }

    private async Task<bool> Validate(string markdownFilePath, Input settings)
    {
        using var _ = _logger.Scope(markdownFilePath);

        var lines = _fileSystem.File.ReadAllLines(markdownFilePath);
        var document = lines.ToMarkdownDocument();
        var links = document.Descendants<LinkInline>();

        var result = true;

        foreach (var link in links)
        {
            using var __ = _logger.Scope($"{link.ToMarkdownString()}; line {link.Line + 1}, column {link.Column}");

            var url = link.Url;

            if (string.IsNullOrWhiteSpace(url))
            {
                _logger.WriteDebug("Skipping empty link.");
                continue;
            }

            if (url.StartsWith("http://") || url.StartsWith("https://"))
            {
                if (!settings.LinkType.HasFlag(LinkType.Remote))
                {
                    _logger.WriteDebug("Skipping remote link.");
                    continue;
                }

                try
                {
                    var response = await _httpClient.Get(url);

                    if (response.IsSuccessStatusCode)
                    {
                        _logger.WriteDebug("Ok");
                    }
                    else
                    {
                        result = false;
                        _logger.WriteError($"{response.StatusCode} {response.ReasonPhrase}");
                    }
                }
                catch (Exception exception)
                {
                    result = false;
                    _logger.WriteError(exception.Message);
                }

                continue;
            }

            if (!settings.LinkType.HasFlag(LinkType.Local))
            {
                _logger.WriteDebug("Skipping local link.");
                continue;
            }

            var anchor = "";
            var linkHasAnchor = url.Contains('#');

            if (linkHasAnchor)
            {
                var index = url.IndexOf('#');

                if (index + 1 == url.Length)
                {
                    linkHasAnchor = false;
                }
                else
                {
                    anchor = url.Substring(index + 1);
                }

                url = index == 0 ? markdownFilePath : url.Substring(0, index);
            }

            if (!_fileSystem.Path.IsPathFullyQualified(url))
            {
                var relativeUrl = url;

                var directory = _fileSystem.Path.GetDirectoryName(markdownFilePath);
                var path = _fileSystem.Path.Combine(directory, relativeUrl);
                url = _fileSystem.Path.GetFullPath(path);

                _logger.WriteDebug($"{relativeUrl} >> {url}");
            }


            if (_fileSystem.File.Exists(url))
            {
                if (linkHasAnchor && url.EndsWith(".md"))
                {
                    var targetDocument = _fileSystem.File.ReadAllLines(url).ToMarkdownDocument();
                    var anchorFound = false;

                    foreach (var heading in targetDocument.Descendants<HeadingBlock>())
                    {
                        var id = heading.TryGetAttributes()?.Id;
                        if (id != anchor) continue;
                        anchorFound = true;
                        break;
                    }

                    if (anchorFound)
                    {
                        _logger.WriteDebug("Ok");
                    }
                    else
                    {
                        result = false;
                        _logger.WriteError("Anchor not found");
                    }
                }
                else
                {
                    _logger.WriteDebug("Ok");
                }
            }
            else
            {
                result = false;
                _logger.WriteError("Not found");
            }
        }

        return result;
    }
}