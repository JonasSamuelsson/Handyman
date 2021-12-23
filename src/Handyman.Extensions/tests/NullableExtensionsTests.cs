namespace Handyman.Extensions.Tests
{
    public class NullableExtensionsTests
    {
        [Fact]
        public void ShouldGetValueOrDefault()
        {
            default(IgnoreCase?).GetValueOrDefault(IgnoreCase.Yes).ShouldBe(IgnoreCase.Yes);
            default(IgnoreCase?).GetValueOrDefault(() => IgnoreCase.Yes).ShouldBe(IgnoreCase.Yes);
        }

        [Fact]
        public void ShouldGetValueOrThrow()
        {
            Should.Throw<InvalidOperationException>(() => default(IgnoreCase?).GetValueOrThrow());
        }

        [Fact]
        public void ShouldCheckIsNull()
        {
            var nullable = default(int?);
            nullable.IsNull().ShouldBe(true);

            nullable = 0;
            nullable.IsNull().ShouldBe(false);
        }

        [Fact]
        public void ShouldCheckIsNotNull()
        {
            var nullable = default(int?);
            nullable.IsNotNull().ShouldBe(false);

            nullable = 0;
            nullable.IsNotNull().ShouldBe(true);
        }
    }
}