using Handyman.Tools.Docs.Shared;
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
            var lines = new[]
            {
                "<handyman-docs:xyz a=\"1\" b=\"2\" />"
            };

            var element = ElementReader.ReadElements(lines).Single();

            element.Attributes.Count().ShouldBe(2);

            element.Attributes.Get("a").ShouldBe("1");
            element.Attributes.Get("b").ShouldBe("2");
        }

        [Fact]
        public void ShouldReadElements()
        {
            var lines = new[]
            {
                "<handyman-docs:abc />",
                "<handyman-docs:xyz />"
            };

            var elements = ElementReader.ReadElements(lines);

            elements.Count.ShouldBe(2);

            elements[0].Name.ShouldBe("abc");
            elements[1].Name.ShouldBe("xyz");
        }

        [Fact]
        public void ShouldThrowIfElementIsNotClosed()
        {
            var lines = new[]
            {
                "<handyman-docs:abc>",
                "<handyman-docs:xyz />"
            };

            Should.Throw<Exception>(() => ElementReader.ReadElements(lines));
        }

        [Fact]
        public void ShouldThrowForUnclosedElement()
        {
            var lines = new[]
            {
                "<handyman-docs:abc>"
            };

            Should.Throw<Exception>(() => ElementReader.ReadElements(lines));
        }

        [Fact]
        public void ShouldIgnoreElementsInsideFencedCodeBlocks()
        {
            var lines = new[]
            {
                "```",
                "<handyman-docs:abc />",
                "```",
                "<handyman-docs:xyz />"
            };

            var element = ElementReader.ReadElements(lines).Single();

            element.Name.ShouldBe("xyz");
        }
    }
}