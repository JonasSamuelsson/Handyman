using Handyman.Tools.Docs.Shared;
using Shouldly;
using Xunit;

namespace Handyman.Tools.Docs.Tests;

public class PatchEngineTests
{
    [Fact]
    public void ShouldApplyPatches()
    {
        var lines = new[]
        {
            "one",
            " // <handyman-docs:abc a=\"1\">",
            "two",
            " // </handyman-docs:abc>",
            "three",
            "/* <handyman-docs:xyz b=\"2\" /> */",
            "four"
        };

        var patches = new[]
        {
            new PatchEngine.Patch
            {
                Content = new[] { "patch 1" },
                Element = new Element
                {
                    Attributes = new Attributes { { "a", "1" } },
                    LineCount = 3,
                    LineIndex = 1,
                    Name = "abc",
                    Postfix = "",
                    Prefix = " // "
                }
            },
            new PatchEngine.Patch
            {
                Content = new[]
                {
                    "patch 2.1",
                    "patch 2.2"
                },
                Element = new Element
                {
                    Attributes = new Attributes { { "b", "2" } },
                    LineCount = 1,
                    LineIndex = 5,
                    Name = "xyz",
                    Postfix = " */",
                    Prefix = "/* "
                }
            }
        };

        var result = PatchEngine.ApplyPatches(lines, patches);

        result.ShouldBe(new[]
        {
            "one",
            " // <handyman-docs:abc a=\"1\">",
            "patch 1",
            " // </handyman-docs:abc>",
            "three",
            "/* <handyman-docs:xyz b=\"2\"> */",
            "patch 2.1",
            "patch 2.2",
            "/* </handyman-docs:xyz> */",
            "four"
        });
    }
}