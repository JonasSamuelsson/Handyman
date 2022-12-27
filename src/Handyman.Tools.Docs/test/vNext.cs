using Handyman.Tools.Docs.Utils;
using Shouldly;
using System;
using System.Linq;
using Xunit;

namespace Handyman.Tools.Docs.Tests
{
    public class ElementReaderTests
    {
        [Fact]
        public void ShouldReadSingleLineElement()
        {
            var lines = new[]
            {
                "<handyman-docs:xyz/>"
            };

            var element = ElementReader.ReadElements(lines).Single();

            element.Content.ShouldBeEmpty();
            element.LineCount.ShouldBe(1);
            element.LineIndex.ShouldBe(0);
            element.Name.ShouldBe("xyz");
        }

        [Fact]
        public void ShouldReadMultiLineElementWithoutContent()
        {
            var lines = new[]
            {
                "<handyman-docs:xyz>",
                "</handyman-docs:xyz>"
            };

            var element = ElementReader.ReadElements(lines).Single();

            element.Content.ShouldBeEmpty();
            element.LineCount.ShouldBe(2);
            element.LineIndex.ShouldBe(0);
            element.Name.ShouldBe("xyz");
        }

        [Fact]
        public void ShouldReadMultiLineElementWithContent()
        {
            var lines = new[]
            {
                "<handyman-docs:xyz>",
                "foo",
                "bar",
                "</handyman-docs:xyz>"
            };

            var element = ElementReader.ReadElements(lines).Single();

            element.Content.ShouldBe(new[] { "foo", "bar" });
            element.LineCount.ShouldBe(4);
            element.LineIndex.ShouldBe(0);
            element.Name.ShouldBe("xyz");
        }

        [Fact]
        public void ShouldReadElementWithAttributes()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ShouldReadElements()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ShouldThrowIfElementIsNotClosed()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ShouldThrowForUnclosedElement()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void ShouldIgnoreElementsInsideFencedCodeBlocks()
        {
            throw new NotImplementedException();
        }
    }

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
                    Element = new E
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
                    Element = new E
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
}