using Handyman.Tools.Docs.ImportCode;
using Handyman.Tools.Docs.Utils;
using Shouldly;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Handyman.Tools.Docs.Tests
{
    public class CodeBlockCommandTests
    {
        private static readonly ElementSerializer<CodeBlockData> CodeBlockSerializer = new ElementSerializer<CodeBlockData>(new DataSerializer<CodeBlockData>());
        private static readonly ElementSerializer<CodeBlockSourceData> CodeBlockSourceSerializer = new ElementSerializer<CodeBlockSourceData>(new DataSerializer<CodeBlockSourceData>());

        [Fact]
        public void ShouldUpdateSingleCodeBlockFromWholeFile()
        {
            var logger = new TestLogger();
            var fileSystem = new MockFileSystem();

            fileSystem.File.WriteAllLines("c:/file.md",
                new[]
                {
                    "before",
                    "<!-- <handyman-docs:code-block source=\"c:/file.txt\" /> -->",
                    "after"
                });

            fileSystem.File.WriteAllLines("c:/file.txt", new[] { "success" });

            new CodeBlocksCommand(logger, fileSystem, CodeBlockSerializer, CodeBlockSourceSerializer)
            {
                Target = "c:/"
            }
                .OnExecute();

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
            var logger = new TestLogger();
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

            new CodeBlocksCommand(logger, fileSystem, CodeBlockSerializer, CodeBlockSourceSerializer)
            {
                Target = "c:/"
            }
                .OnExecute();

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
            var logger = new TestLogger();
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

            new CodeBlocksCommand(logger, fileSystem, CodeBlockSerializer, CodeBlockSourceSerializer)
            {
                Target = "c:/"
            }
                .OnExecute();

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
            var logger = new TestLogger();
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

            new CodeBlocksCommand(logger, fileSystem, CodeBlockSerializer, CodeBlockSourceSerializer)
            {
                Target = "c:/"
            }
                .OnExecute();

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
    }
}
