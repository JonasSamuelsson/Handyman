using Handyman.Wpf.Converters;
using Shouldly;

namespace Handyman.Tests.Wpf.Converters
{
    public class InvertBoolConverterTests
    {
        public void FalseShouldConvertToTrue()
        {
            new InvertBoolConverter().Convert(false, null, null, null).ShouldBe(true);
        }

        public void FalseShouldConvertBackToTrue()
        {
            new InvertBoolConverter().ConvertBack(false, null, null, null).ShouldBe(true);
        }

        public void TrueShouldConvertToFalse()
        {
            new InvertBoolConverter().Convert(false, null, null, null).ShouldBe(true);
        }

        public void TrueShouldConvertBackToFalse()
        {
            new InvertBoolConverter().ConvertBack(false, null, null, null).ShouldBe(true);
        }
    }
}