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
    [InlineData("../found.md", 0)]
    [InlineData("../not-found.md", 1)]
    [InlineData("c:/found.md", 0)]
    [InlineData("c:/not-found.md", 1)]
    public async Task ShouldValidateLocalLinks(string link, int exitCode)
    {
        var fileSystem = new MockFileSystem();

        fileSystem.File.WriteAllLines("c:/temp/file.md", new[]
        {
            $"[text]({link})"
        });

        fileSystem.File.WriteAllLines("c:/found.md", new string[] { });

        var command = new ValidateLinksCommand(fileSystem, new TestLogger(), new TestHttpClient());

        var input = new ValidateLinksCommand.Input
        {
            ExitCode = true,
            LinkType = LinkType.Local,
            TargetPath = "c:/"
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
}