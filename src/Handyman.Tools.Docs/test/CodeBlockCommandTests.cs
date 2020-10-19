using Handyman.Tools.Docs.ImportCode;
using Handyman.Tools.Docs.Utils;
using Shouldly;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Handyman.Tools.Docs.Tests
{
    public class CodeBlockCommandTests
    {
        [Fact]
        public void ShouldUpdateCodeBlock()
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

            new ImportCodeCommand(logger, fileSystem, new ElementSerializer<CodeBlockData>(new DataSerializer<CodeBlockData>()))
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
    }
}
