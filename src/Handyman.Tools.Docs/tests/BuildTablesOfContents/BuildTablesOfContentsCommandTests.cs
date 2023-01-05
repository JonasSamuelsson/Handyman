using Handyman.Tools.Docs.BuildTablesOfContents;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shouldly;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Xunit;

namespace Handyman.Tools.Docs.Tests.BuildTablesOfContents;

public class BuildTablesOfContentsCommandTests
{
    [Fact]
    public void ShouldHandleSimpleCase()
    {
        var lines = new[]
        {
            "# foo"
        };

        var tableOfContent = BuildTablesOfContentsCommand.GenerateTableOfContent(lines, new TableOfContentsAttributes());

        tableOfContent.ShouldBe(new[]
        {
            "- [foo](#foo)"
        });
    }

    [Fact]
    public void ShouldHandleHeadingTextDuplicates()
    {
        var lines = new[]
        {
            "# foo",
            "# foo"
        };

        var tableOfContent = BuildTablesOfContentsCommand.GenerateTableOfContent(lines, new TableOfContentsAttributes());

        tableOfContent.ShouldBe(new[]
        {
            "- [foo](#foo)",
            "- [foo](#foo-1)"
        });
    }

    [Fact]
    public void ShouldHandleTextWithSpecialCharacters()
    {
        var lines = new[]
        {
            "# foo & bar"
        };

        var tableOfContent = BuildTablesOfContentsCommand.GenerateTableOfContent(lines, new TableOfContentsAttributes());

        tableOfContent.ShouldBe(new[]
        {
            "- [foo & bar](#foo-bar)"
        });
    }

    [Fact]
    public void ShouldHandleTextWithFormatting()
    {
        var lines = new[]
        {
            "# `foo` *bar*"
        };

        var tableOfContent = BuildTablesOfContentsCommand.GenerateTableOfContent(lines, new TableOfContentsAttributes());

        tableOfContent.ShouldBe(new[]
        {
            "- [`foo` *bar*](#foo-bar)"
        });
    }

    [Fact]
    public void ShouldHandleMultipleLevels()
    {
        var lines = new[]
        {
            "# foo",
            "## bar",
            "### foo bar",
            "# foobar"
        };

        var tableOfContent = BuildTablesOfContentsCommand.GenerateTableOfContent(lines, new TableOfContentsAttributes());

        tableOfContent.ShouldBe(new[]
        {
            "- [foo](#foo)",
            "  - [bar](#bar)",
            "    - [foo bar](#foo-bar)",
            "- [foobar](#foobar)"
        });
    }

    [Fact]
    public void ShouldHandleThatH1IsNotTopLevel()
    {
        var lines = new[]
        {
            "## foo",
            "### bar"
        };

        var tableOfContent = BuildTablesOfContentsCommand.GenerateTableOfContent(lines, new TableOfContentsAttributes());

        tableOfContent.ShouldBe(new[]
        {
            "- [foo](#foo)",
            "  - [bar](#bar)"
        });
    }

    [Fact]
    public void ShouldHandleOutOfOrderLevels()
    {
        var lines = new[]
        {
            "## foo",
            "# bar",
            "### foobar"
        };

        var tableOfContent = BuildTablesOfContentsCommand.GenerateTableOfContent(lines, new TableOfContentsAttributes());

        tableOfContent.ShouldBe(new[]
        {
            "  - [foo](#foo)",
            "- [bar](#bar)",
            "    - [foobar](#foobar)"
        });
    }

    [Fact]
    public void ShouldSupportExplicitLevels()
    {
        var lines = new[]
        {
            "# foo",
            "## bar",
            "### foo bar",
            "# foobar"
        };

        var tableOfContent = BuildTablesOfContentsCommand.GenerateTableOfContent(lines, new TableOfContentsAttributes
        {
            Levels = new[] { 2, 3 }
        });

        tableOfContent.ShouldBe(new[]
        {
            "- [bar](#bar)",
            "  - [foo bar](#foo-bar)"
        });
    }

    [Fact]
    public void ShouldSupportOrderedLists()
    {
        var lines = new[]
        {
            "# foo",
            "## bar",
            "### foo bar",
            "# foobar"
        };

        var tableOfContent = BuildTablesOfContentsCommand.GenerateTableOfContent(lines, new TableOfContentsAttributes
        {
            ListType = ListType.Ordered
        });

        tableOfContent.ShouldBe(new[]
        {
            "0. [foo](#foo)",
            "  0. [bar](#bar)",
            "    0. [foo bar](#foo-bar)",
            "0. [foobar](#foobar)"
        });
    }

    [Fact]
    public void EndToEndTest()
    {
        var fileSystem = new MockFileSystem();

        fileSystem.File.WriteAllLines("c:/main.md".FixOsNeutralPaths(), new[]
        {
            "<!--<handyman-docs:table-of-content />-->",
            "# one",
            "## two",
            "<!--<handyman-docs:table-of-content SourcePath=\"extra.md\" ListType=\"ordered\" />-->",
        });

        fileSystem.File.WriteAllLines("c:/extra.md".FixOsNeutralPaths(), new[]
        {
            "# xyz",
        });

        var exitCode = Program.Run(new[] { "build-tables-of-contents", "c:/main.md".FixOsNeutralPaths() }, services => services.Replace(new ServiceDescriptor(typeof(IFileSystem), fileSystem)));

        exitCode.ShouldBe(0);

        fileSystem.File.ReadAllLines("c:/main.md".FixOsNeutralPaths()).ShouldBe(new[]
        {
            "<!--<handyman-docs:table-of-content>-->",
            "- [one](#one)",
            "  - [two](#two)",
            "<!--</handyman-docs:table-of-content>-->",
            "# one",
            "## two",
            "<!--<handyman-docs:table-of-content SourcePath=\"extra.md\" ListType=\"ordered\">-->",
            "0. [xyz](#xyz)",
            "<!--</handyman-docs:table-of-content>-->"
        });
    }
}