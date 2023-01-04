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

            fileSystem.File.WriteAllLines("c:/file.md",
                new[]
                {
                    "before",
                    "<!-- <handyman-docs:code-block source=\"c:/file.txt\" /> -->",
                    "after"
                });

            fileSystem.File.WriteAllLines("c:/file.txt", new[] { "success" });

            var elementReader = new ElementReader();
            var attributesConverter = new AttributesConverter(ArraySegment<IValueConverter>.Empty);

            new ImportCodeBlocksCommand(fileSystem, new TestLogger(), elementReader, attributesConverter)
                .Execute(new ImportCodeBlocksCommand.Input
                {
                    TargetPath = "c:/"
                });

            fileSystem.File.ReadAllLines("c:/file.md")
                .ShouldBe(new[]
                {
                    "before",
                    "<!-- <handyman-docs:code-block source=\"c:/file.txt\"> -->",
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

            fileSystem.File.WriteAllLines("c:/file.md",
                new[]
                {
                    "before",
                    "<!-- <handyman-docs:code-block source=\"c:/file.txt\" lines=\"2\" /> -->",
                    "after"
                });

            fileSystem.File.WriteAllLines("c:/file.txt",
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
                    "<!-- <handyman-docs:code-block source=\"c:/file.txt\" lines=\"2\"> -->",
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

            fileSystem.File.WriteAllLines("c:/file.md",
                new[]
                {
                    "before",
                    "<!-- <handyman-docs:code-block source=\"c:/file.txt\" id=\"foo\" /> -->",
                    "after"
                });

            fileSystem.File.WriteAllLines("c:/file.txt",
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
                    TargetPath = "c:/"
                });

            fileSystem.File.ReadAllLines("c:/file.md")
                .ShouldBe(new[]
                {
                    "before",
                    "<!-- <handyman-docs:code-block source=\"c:/file.txt\" id=\"foo\"> -->",
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
                    "<!-- <handyman-docs:code-block source=\"c:/file1.txt\" /> -->",
                    "between",
                    "<!-- <handyman-docs:code-block source=\"c:/file2.txt\" /> -->",
                    "after"
                });

            fileSystem.File.WriteAllLines("c:/file1.txt", new[] { "one" });
            fileSystem.File.WriteAllLines("c:/file2.txt", new[] { "two" });

            var elementReader = new ElementReader();
            var attributesConverter = new AttributesConverter(ArraySegment<IValueConverter>.Empty);

            new ImportCodeBlocksCommand(fileSystem, new TestLogger(), elementReader, attributesConverter)
                .Execute(new ImportCodeBlocksCommand.Input
                {
                    TargetPath = "c:/"
                });

            fileSystem.File.ReadAllLines("c:/file.md")
                .ShouldBe(new[]
                {
                    "before",
                    "<!-- <handyman-docs:code-block source=\"c:/file1.txt\"> -->",
                    "```txt",
                    "one",
                    "```",
                    "<!-- </handyman-docs:code-block> -->",
                    "between",
                    "<!-- <handyman-docs:code-block source=\"c:/file2.txt\"> -->",
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

            fileSystem.AddFile(@"x:\dir\readme.md", new[]
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
            fileSystem.AddFile(@"x:\dir\a.cs", new[]
            {
                "public class A { }"
            });
            fileSystem.AddFile(@"x:\dir\b.txt", new[]
            {
                "first",
                "second",
                "third"
            });
            fileSystem.AddFile(@"x:\dir\c.ps1", new[]
            {
                "write-host 1",
                "write-host 2",
                "write-host 3",
                "write-host 4",
                "write-host 5"
            });

            var exitCode = Program.Run(new[] { "import-code-blocks", @"x:\dir" }, services => services.Replace(new ServiceDescriptor(typeof(IFileSystem), fileSystem)));

            exitCode.ShouldBe(0);

            fileSystem.File.ReadAllLines(@"x:\dir\readme.md").ShouldBe(new[]
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