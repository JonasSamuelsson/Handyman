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

            CodeBlocksCommand.AddSyntaxHighlighting(lines, "csharp");

            lines.ShouldBe(new[]
            {
                "```csharp",
                "foo",
                "```"
            });
        }
    }
}