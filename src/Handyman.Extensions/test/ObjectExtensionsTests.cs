using Shouldly;
using Xunit;

namespace Handyman.Extensions.Tests
{
    public class ObjectExtensionsTests
    {
        [Fact]
        public void ShouldCheckIsNull()
        {
            default(object).IsNull().ShouldBe(true);
            new object().IsNull().ShouldBe(false);
        }

        [Fact]
        public void ShouldCheckIsNotNull()
        {
            default(object).IsNotNull().ShouldBe(false);
            new object().IsNotNull().ShouldBe(true);
        }

        [Fact]
        public void Coalesce()
        {
            2.Coalesce(i => i > 1, 3).ShouldBe(2);
            2.Coalesce(i => i < 1, 3).ShouldBe(3);
            2.Coalesce(i => i > 1, () => 3).ShouldBe(2);
            2.Coalesce(i => i < 1, () => 3).ShouldBe(3);
            2.Coalesce(i => i > 1, i => i + 1).ShouldBe(2);
            2.Coalesce(i => i < 1, i => i + 1).ShouldBe(3);
        }
    }
}
