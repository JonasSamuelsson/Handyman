using Handyman.Tools.Docs.ImportCode;
using Shouldly;
using System.Collections.Generic;
using Xunit;

namespace Handyman.Tools.Docs.Tests
{
    public class ImportCodeCommand_AddSyntaxHighlightingTests
    {
        [Fact]
        public void ShouldAddSyntaxHighlighting()
        {
            var lines = new List<string> { "foo" };

            ImportCodeCommand.AddSyntaxHighlighting(lines, ".cs");

            lines.ShouldBe(new[]
            {
                "```csharp",
                "foo",
                "```"
            });
        }

        [Fact]
        public void ShouldUseExtensionAsLanguageIfNotKnown()
        {
            var lines = new List<string> { "foo" };

            ImportCodeCommand.AddSyntaxHighlighting(lines, ".iowrweuior");

            lines.ShouldBe(new[]
            {
                "```iowrweuior",
                "foo",
                "```"
            });
        }
    }
}