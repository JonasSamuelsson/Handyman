using Shouldly;

namespace Handyman.Tests.Core
{
    public class TempTests
    {
        public void ShouldModifyAndRestore()
        {
            var number = 0;
            using (new Temp(() => number++, () => number--))
                number.ShouldBe(1);
            number.ShouldBe(0);
        }
    }
}