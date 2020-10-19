using Handyman.Tools.Docs.Utils;
using Shouldly;
using Xunit;

namespace Handyman.Tools.Docs.Tests.Utils
{
    public class DefaultValueConverterTests
    {
        [Fact]
        public void ShouldConvertString()
        {
            new DefaultValueConverter<string>()
                .TryConvertFromString("success", new TestLogger(), out var value)
                .ShouldBeTrue();

            value.ShouldBe("success");
        }
    }
}