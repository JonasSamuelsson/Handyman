using Handyman.Tools.Docs.ValidateLinks;
using Shouldly;
using System;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Tools.Docs.Tests.ValidateLinks;

public class ValidateLinksCommandTests
{
    [Theory]
    [InlineData(@"C:\a\file.md", @"found.md", 0)]
    [InlineData(@"C:\a\file.md", @".\found.md", 0)]
    [InlineData(@"C:\a\file.md", @"..\not-found.md", 1)]
    [InlineData(@"C:\a\file.md", @"C:\a\found.md", 0)]
    [InlineData(@"C:\a\file.md", @"C:\a\not-found.md", 1)]
    [InlineData(@"C:\a\b\file.md", @"\found.md", 0)]
    public async Task ShouldValidateLocalLinks(string filePath, string link, int exitCode)
    {
        var fileSystem = new MockFileSystem();

        fileSystem.Directory.CreateDirectory(@"C:\a");
        fileSystem.Directory.CreateDirectory(@"C:\a\.git");

        fileSystem.AddFile(@"C:\a\found.md", new string[] { });

        fileSystem.AddFile(filePath, new[]
        {
            $"[text]({link})"
        });

        var command = new ValidateLinksCommand(fileSystem, new TestLogger(), new TestHttpClient());

        var input = new ValidateLinksCommand.Input
        {
            ExitCode = true,
            LinkType = LinkType.Local,
            TargetPath = @"C:\"
        };

        (await command.Execute(input)).ShouldBe(exitCode);
    }

    [Theory]
    [InlineData("http://found.com", 0)]
    [InlineData("http://not-found.com", 1)]
    [InlineData("http://throw", 1)]
    public async Task ShouldValidateRemoteLinks(string link, int exitCode)
    {
        var fileSystem = new MockFileSystem();

        fileSystem.File.WriteAllLines("c:/file.md", new[]
        {
            $"[text]({link})"
        });

        var httpClient = new TestHttpClient
        {
            Handler = url => new Response
            {
                StatusCode = url switch
                {
                    "http://found.com" => 200,
                    "http://not-found.com" => 404,
                    _ => throw new Exception()
                }
            }
        };

        var command = new ValidateLinksCommand(fileSystem, new TestLogger(), httpClient);

        var input = new ValidateLinksCommand.Input
        {
            ExitCode = true,
            LinkType = LinkType.Remote,
            TargetPath = "c:/"
        };

        (await command.Execute(input)).ShouldBe(exitCode);
    }

    [Theory]
    [InlineData("#", 0)]
    [InlineData("#parent", 0)]
    [InlineData("#child", 1)]
    [InlineData("file.md#", 0)]
    [InlineData("file.md#parent", 0)]
    [InlineData("file.md#child", 1)]
    [InlineData("temp/file.md#", 0)]
    [InlineData("temp/file.md#parent", 1)]
    [InlineData("temp/file.md#child", 0)]
    public async Task ShouldHandleMarkdownAnchorLinks(string link, int exitCode)
    {
        var fileSystem = new MockFileSystem();

        fileSystem.File.WriteAllLines("c:/file.md", new[]
        {
            $"[text]({link})",
            "# Parent",
            "text"
        });

        fileSystem.File.WriteAllLines("c:/temp/file.md", new[]
        {
            "# Child",
            "text"
        });

        var command = new ValidateLinksCommand(fileSystem, new TestLogger(), new TestHttpClient());

        var input = new ValidateLinksCommand.Input
        {
            ExitCode = true,
            LinkType = LinkType.Local,
            TargetPath = "c:/"
        };

        (await command.Execute(input)).ShouldBe(exitCode);
    }
}