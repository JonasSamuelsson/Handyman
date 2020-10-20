using Handyman.Tools.Docs.Utils;
using Shouldly;
using System.Linq;
using Xunit;

namespace Handyman.Tools.Docs.Tests.Utils
{
    public class LinesValueConverterTests
    {
        [Fact]
        public void ShouldConvertFromSingleLine()
        {
            new LinesValueConverter()
                .TryConvertFromString("5", new TestLogger(), out var lines)
                .ShouldBeTrue();

            lines.Count.ShouldBe(1);
            lines.FromIndex.ShouldBe(4);
            lines.FromNumber.ShouldBe(5);
            lines.Text.ShouldBe("5");
        }

        [Fact]
        public void ShouldConvertFromRange()
        {
            new LinesValueConverter()
                .TryConvertFromString("4-6", new TestLogger(), out var lines)
                .ShouldBeTrue();

            lines.Count.ShouldBe(3);
            lines.FromIndex.ShouldBe(3);
            lines.FromNumber.ShouldBe(4);
            lines.Text.ShouldBe("4-6");
        }

        [Fact]
        public void ShouldConvertFromBlock()
        {
            new LinesValueConverter()
                .TryConvertFromString("6+3", new TestLogger(), out var lines)
                .ShouldBeTrue();

            lines.Count.ShouldBe(4);
            lines.FromIndex.ShouldBe(5);
            lines.FromNumber.ShouldBe(6);
            lines.Text.ShouldBe("6+3");
        }

        [Theory]
        [InlineData("-1", "e:Invalid format '-1', value can't be less than 1.")]
        [InlineData("0", "e:Invalid format '0', value can't be less than 1.")]
        [InlineData("x", "e:Invalid format 'x'.")]
        [InlineData("4-3", "e:Invalid format '4-3', from can't greater than to.")]
        [InlineData("4-x", "e:Invalid format '4-x'.")]
        [InlineData("4+-1", "e:Invalid format '4+-1', count can't be less than 1.")]
        [InlineData("4+x", "e:Invalid format '4+x'.")]
        public void ShouldNotConvert(string s, string message)
        {
            var logger = new TestLogger();

            new LinesValueConverter()
                .TryConvertFromString(s, logger, out _)
                .ShouldBeFalse();

            logger.Messages.Single().ShouldBe(message);
        }
    }
}