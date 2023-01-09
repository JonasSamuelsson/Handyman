using Handyman.Tools.Docs.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Handyman.Tools.Docs.Tests.ImportContent;

public class ImportContentCommandTests
{
    [Fact]
    public void ShouldImportContentEndToEnd()
    {
        var fileSystem = new MockFileSystem();

        fileSystem.File.WriteAllLines("c:/readme.md", new[]
        {
            "one",
            "<!-- <handyman-docs:content source=\"c:/temp/file.txt\" id=\"xyz\"> -->",
            "two",
            "<!-- </handyman-docs:content> -->",
            "three"
        });

        fileSystem.File.WriteAllLines("c:/temp/file.txt", new[]
        {
            "ONE",
            "# <handyman-docs:content-source id=\"xyz\" lines=\"3\" />",
            "THREE",
            "FOUR"
        });

        var exitCode = Program.Run(new[] { "import-content", "c:/" }, services =>
        {
            services.Replace(new ServiceDescriptor(typeof(IFileSystem), fileSystem));
            services.Replace(new ServiceDescriptor(typeof(ILogger), typeof(TestLogger), ServiceLifetime.Singleton));
        });

        exitCode.ShouldBe(0);

        fileSystem.File.ReadAllLines("c:/readme.md").ShouldBe(new[]
        {
            "one",
            "<!-- <handyman-docs:content source=\"c:/temp/file.txt\" id=\"xyz\"> -->",
            "THREE",
            "<!-- </handyman-docs:content> -->",
            "three"
        });
    }
}