using System.Collections.Generic;
using Handyman.Tools.Docs.ImportCode;
using Shouldly;
using Xunit;

namespace Handyman.Tools.Docs.Tests
{
    public class ImportCodeCommand_TrimIndentationTests
    {
        [Fact]
        public void ShouldTrimLeadingSpaces()
        {
            var lines = new List<string>
            {
                " foo",
                "  bar"
            };

            CodeBlocksCommand.TrimIndentation(lines);

            lines.ShouldBe(new[]
            {
                "foo",
                " bar"
            });
        }

        [Fact]
        public void ShouldTrimLeadingTabs()
        {
            var lines = new List<string>
            {
                "\tfoo",
                "\t\tbar"
            };

            CodeBlocksCommand.TrimIndentation(lines);

            lines.ShouldBe(new[]
            {
                "foo",
                "\tbar"
            });
        }

        [Fact]
        public void ShouldHandleSpacesAndTabs()
        {
            var lines = new List<string>
            {
                " foo",
                "\tbar"
            };

            CodeBlocksCommand.TrimIndentation(lines);

            lines.ShouldBe(new[]
            {
                " foo",
                "\tbar"
            });
        }

        [Fact]
        public void ShouldHandleEmptyAndWhitespaceLines()
        {
            var lines = new List<string>
            {
                " foo",
                "\t",
                "  bar",
                " ",
                "   bazz",
                ""
            };

            CodeBlocksCommand.TrimIndentation(lines);

            lines.ShouldBe(new[]
            {
                "foo",
                "",
                " bar",
                "",
                "  bazz",
                ""
            });
        }
    }
}