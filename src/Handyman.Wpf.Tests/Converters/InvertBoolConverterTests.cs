using Handyman.Wpf.Converters;
using Shouldly;
using Xunit;

namespace Handyman.Wpf.Tests.Converters
{
    public class InvertBoolConverterTests
    {
        [Fact]
        public void FalseShouldConvertToTrue()
        {
            new InvertBoolConverter().Convert(false, null, null, null).ShouldBe(true);
        }

        [Fact]
        public void FalseShouldConvertBackToTrue()
        {
            new InvertBoolConverter().ConvertBack(false, null, null, null).ShouldBe(true);
        }

        [Fact]
        public void TrueShouldConvertToFalse()
        {
            new InvertBoolConverter().Convert(false, null, null, null).ShouldBe(true);
        }

        [Fact]
        public void TrueShouldConvertBackToFalse()
        {
            new InvertBoolConverter().ConvertBack(false, null, null, null).ShouldBe(true);
        }
    }
}