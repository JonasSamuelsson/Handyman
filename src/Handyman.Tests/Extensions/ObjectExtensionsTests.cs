using Handyman.Extensions;
using Shouldly;
using Xunit;

namespace Handyman.Tests.Extensions
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
    }
}
