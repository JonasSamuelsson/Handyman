using Handyman.Tools.Docs.ImportCodeBlocks;
using Handyman.Tools.Docs.Shared;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using System;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Handyman.Tools.Docs.Tests.ImportCodeBlocks
{
    public class ImportCodeBlocksCommandTests
    {
        [Fact]
        public void ShouldUpdateSingleCodeBlockFromWholeFile()
        {
            var fileSystem = new MockFileSystem();

            fileSystem.File.WriteAllLines("c:/file.md".FixOsNeutralPaths(),
                new[]
                {
                    "before",
                    "<!-- <handyman-docs:code-block source=\"c:/file.txt\" /> -->".FixOsNeutralPaths(),
                    "after"
                });

            fileSystem.File.WriteAllLines("c:/file.txt", new[] { "success" });

            var elementReader = new ElementReader();
            var attributesConverter = new AttributesConverter(ArraySegment<IValueConverter>.Empty);

            new ImportCodeBlocksCommand(fileSystem, new TestLogger(), elementReader, attributesConverter)
                .Execute(new ImportCodeBlocksCommand.Input
                {
                    TargetPath = "c:/".FixOsNeutralPaths()
                });

            fileSystem.File.ReadAllLines("c:/file.md".FixOsNeutralPaths())
                .ShouldBe(new[]
                {
                    "before",
                    "<!-- <handyman-docs:code-block source=\"c:/file.txt\"> -->".FixOsNeutralPaths(),
                    "```txt",
                    "success",
                    "```",
                    "<!-- </handyman-docs:code-block> -->",
                    "after"
                });
        }

        [Fact]
        public void ShouldUpdateSingleCodeBlockWithSourceLines()
        {
            var fileSystem = new MockFileSystem();

            fileSystem.File.WriteAllLines("c:/file.md".FixOsNeutralPaths(),
                new[]
                {
                    "before",
                    "<!-- <handyman-docs:code-block source=\"c:/file.txt\" lines=\"2\" /> -->".FixOsNeutralPaths(),
                    "after"
                });

            fileSystem.File.WriteAllLines("c:/file.txt".FixOsNeutralPaths(),
                new[]
                {
                    "first",
                    "second",
                    "third"
                });

            var elementReader = new ElementReader();
            var attributesConverter = new AttributesConverter(new[] { new LinesSpecValueConverter() });

            new ImportCodeBlocksCommand(fileSystem, new TestLogger(), elementReader, attributesConverter)
                .Execute(new ImportCodeBlocksCommand.Input
                {
                    TargetPath = "c:/"
                });

            fileSystem.File.ReadAllLines("c:/file.md")
                .ShouldBe(new[]
                {
                    "before",
                    "<!-- <handyman-docs:code-block source=\"c:/file.txt\" lines=\"2\"> -->".FixOsNeutralPaths(),
                    "```txt",
                    "second",
                    "```",
                    "<!-- </handyman-docs:code-block> -->",
                    "after"
                });
        }

        [Fact]
        public void ShouldUpdateSingleCodeBlockWithSourceId()
        {
            var fileSystem = new MockFileSystem();

            fileSystem.File.WriteAllLines("c:/file.md".FixOsNeutralPaths(),
                new[]
                {
                    "before",
                    "<!-- <handyman-docs:code-block source=\"c:/file.txt\" id=\"foo\" /> -->".FixOsNeutralPaths(),
                    "after"
                });

            fileSystem.File.WriteAllLines("c:/file.txt".FixOsNeutralPaths(),
                new[]
                {
                    "first",
                    "<handyman-docs:code-block-source id=\"foo\">",
                    "second",
                    "</handyman-docs:code-block-source>",
                    "third"
                });

            var elementReader = new ElementReader();
            var attributesConverter = new AttributesConverter(ArraySegment<IValueConverter>.Empty);

            new ImportCodeBlocksCommand(fileSystem, new TestLogger(), elementReader, attributesConverter)
                .Execute(new ImportCodeBlocksCommand.Input
                {
                    TargetPath = "c:/".FixOsNeutralPaths()
                });

            fileSystem.File.ReadAllLines("c:/file.md".FixOsNeutralPaths())
                .ShouldBe(new[]
                {
                    "before",
                    "<!-- <handyman-docs:code-block source=\"c:/file.txt\" id=\"foo\"> -->".FixOsNeutralPaths(),
                    "```txt",
                    "second",
                    "```",
                    "<!-- </handyman-docs:code-block> -->",
                    "after"
                });
        }

        [Fact]
        public void ShouldUpdateMultipleCodeBlocks()
        {
            var fileSystem = new MockFileSystem();

            fileSystem.File.WriteAllLines("c:/file.md",
                new[]
                {
                    "before",
                    "<!-- <handyman-docs:code-block source=\"c:/file1.txt\" /> -->".FixOsNeutralPaths(),
                    "between",
                    "<!-- <handyman-docs:code-block source=\"c:/file2.txt\" /> -->".FixOsNeutralPaths(),
                    "after"
                });

            fileSystem.File.WriteAllLines("c:/file1.txt".FixOsNeutralPaths(), new[] { "one" });
            fileSystem.File.WriteAllLines("c:/file2.txt".FixOsNeutralPaths(), new[] { "two" });

            var elementReader = new ElementReader();
            var attributesConverter = new AttributesConverter(ArraySegment<IValueConverter>.Empty);

            new ImportCodeBlocksCommand(fileSystem, new TestLogger(), elementReader, attributesConverter)
                .Execute(new ImportCodeBlocksCommand.Input
                {
                    TargetPath = "c:/".FixOsNeutralPaths()
                });

            fileSystem.File.ReadAllLines("c:/file.md".FixOsNeutralPaths())
                .ShouldBe(new[]
                {
                    "before",
                    "<!-- <handyman-docs:code-block source=\"c:/file1.txt\"> -->".FixOsNeutralPaths(),
                    "```txt",
                    "one",
                    "```",
                    "<!-- </handyman-docs:code-block> -->",
                    "between",
                    "<!-- <handyman-docs:code-block source=\"c:/file2.txt\"> -->".FixOsNeutralPaths(),
                    "```txt",
                    "two",
                    "```",
                    "<!-- </handyman-docs:code-block> -->",
                    "after"
                });
        }

        [Fact]
        public void EndToEndTest()
        {
            var fileSystem = new MockFileSystem();

            fileSystem.AddFile("c:/dir/readme.md", new[]
            {
                "1",
                "<!-- <handyman-docs:code-block source=\"a.cs\" /> -->",
                "2",
                "<!-- <handyman-docs:code-block source=\"b.txt\" lines=\"2\" /> -->",
                "3",
                "<!-- <handyman-docs:code-block source=\"c.ps1\" lines=\"2-4\"> -->",
                "4",
                "<!-- </handyman-docs:code-block> -->",
                "5"
            });
            fileSystem.AddFile("c:/dir/a.cs".FixOsNeutralPaths(), new[]
            {
                "public class A { }"
            });
            fileSystem.AddFile("c:/dir/b.txt".FixOsNeutralPaths(), new[]
            {
                "first",
                "second",
                "third"
            });
            fileSystem.AddFile("c:/dir/c.ps1".FixOsNeutralPaths(), new[]
            {
                "write-host 1",
                "write-host 2",
                "write-host 3",
                "write-host 4",
                "write-host 5"
            });

            var exitCode = Program.Run(new[] { "import-code-blocks", "c:/dir".FixOsNeutralPaths() }, services =>
            {
                services.Replace(new ServiceDescriptor(typeof(IFileSystem), fileSystem));
                services.Replace(new ServiceDescriptor(typeof(ILogger), typeof(TestLogger), ServiceLifetime.Singleton));
            });

            exitCode.ShouldBe(0);

            fileSystem.File.ReadAllLines("c:/dir/readme.md".FixOsNeutralPaths()).ShouldBe(new[]
            {
                "1",
                "<!-- <handyman-docs:code-block source=\"a.cs\"> -->",
                "```csharp",
                "public class A { }",
                "```",
                "<!-- </handyman-docs:code-block> -->",
                "2",
                "<!-- <handyman-docs:code-block source=\"b.txt\" lines=\"2\"> -->",
                "```txt",
                "second",
                "```",
                "<!-- </handyman-docs:code-block> -->",
                "3",
                "<!-- <handyman-docs:code-block source=\"c.ps1\" lines=\"2-4\"> -->",
                "```powershell",
                "write-host 2",
                "write-host 3",
                "write-host 4",
                "```",
                "<!-- </handyman-docs:code-block> -->",
                "5"
            });
        }
    }
}