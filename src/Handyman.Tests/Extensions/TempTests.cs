using Handyman.Extensions;
using Shouldly;
using Xunit;

namespace Handyman.Tests.Extensions
{
    public class TempTests
    {
        [Fact]
        public void ShouldModifyAndRestore()
        {
            var number = 0;
            using (new Temp(() => number++, () => number--))
                number.ShouldBe(1);
            number.ShouldBe(0);
        }
    }
}