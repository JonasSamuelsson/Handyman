using Handyman.Tools.Docs.TableOfContent;
using Handyman.Tools.Docs.Utils;
using Shouldly;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using Xunit;
using Attributes = Handyman.Tools.Docs.TableOfContent.Attributes;

namespace Handyman.Tools.Docs.Tests
{
    public class TableOfContentCommandTests
    {
        [Fact]
        public void ShouldHandleSimpleCase()
        {
            var lines = new[]
            {
                "# foo"
            };

            var tableOfContent = TableOfContentCommand.GenerateTableOfContent(lines, new Attributes());

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

            var tableOfContent = TableOfContentCommand.GenerateTableOfContent(lines, new Attributes());

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

            var tableOfContent = TableOfContentCommand.GenerateTableOfContent(lines, new Attributes());

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

            var tableOfContent = TableOfContentCommand.GenerateTableOfContent(lines, new Attributes());

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

            var tableOfContent = TableOfContentCommand.GenerateTableOfContent(lines, new Attributes());

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

            var tableOfContent = TableOfContentCommand.GenerateTableOfContent(lines, new Attributes());

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

            var tableOfContent = TableOfContentCommand.GenerateTableOfContent(lines, new Attributes());

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

            var tableOfContent = TableOfContentCommand.GenerateTableOfContent(lines, new Attributes
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

            var tableOfContent = TableOfContentCommand.GenerateTableOfContent(lines, new Attributes
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
        public void ShouldHandleEndToEndScenario()
        {
            var fileSystem = new MockFileSystem();

            fileSystem.File.WriteAllLines("c:/temp/main.md", new[]
            {
                "<!--<handyman-docs:table-of-content />-->",
                "# one",
                "## two",
                "<!--<handyman-docs:table-of-content SourcePath=\"extra.md\" ListType=\"ordered\" />-->",
            });

            fileSystem.File.WriteAllLines("c:/temp/extra.md", new[]
            {
                "# xyz",
            });

            var command = new TableOfContentCommand(fileSystem, new ElementReader(), new AttributesConverter(new[] { new EnumValueConverter() }));

            command.Execute(new TableOfContentCommand.Input
            {
                TargetPath = "c:/temp/main.md"
            });

            fileSystem.File.ReadAllLines("c:/temp/main.md").ShouldBe(new[]
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

    //public class TestFileSystem : IFileSys
    //{
    //    private readonly Dictionary<string, List<string>> _dictionary = new();

    //    public string GetFullPath(string path)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public bool DirectoryExists(string path)
    //    {
    //        return _dictionary.Keys.Any(x => x.StartsWith(path));
    //    }

    //    public bool FileExists(string path)
    //    {
    //        return _dictionary.ContainsKey(path);
    //    }

    //    public IReadOnlyList<string> ListFiles(string path, string pattern)
    //    {
    //        return _dictionary.Keys
    //            .Where(x => x.StartsWith(path))
    //            .ToList();
    //    }

    //    public IReadOnlyList<string> ReadFile(string path)
    //    {
    //        return _dictionary[path];
    //    }

    //    public void WriteFile(string path, IEnumerable<string> lines)
    //    {
    //        _dictionary[path] = lines.ToList();
    //    }
    //}

    public static class CommandExtensions
    {
        public static int Execute<T>(this Command<T> command, T input) where T : CommandSettings
        {
            var commandContext = new CommandContext(new RemainingArguments(), "", null);

            return command.Execute(commandContext, input);
        }

        private class RemainingArguments : IRemainingArguments
        {
            public ILookup<string, string> Parsed { get; } = Array.Empty<string>().ToLookup(x => x);
            public IReadOnlyList<string> Raw { get; } = Array.Empty<string>();
        }
    }
}