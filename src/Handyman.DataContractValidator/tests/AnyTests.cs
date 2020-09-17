using Xunit;

namespace Handyman.DataContractValidator.Tests
{
    public class AnyTests
    {
        [Fact]
        public void ShouldSupportPropertiesOfAnyType()
        {
            new DataContractValidator().Validate(typeof(Actual), new { Value = typeof(Any) });
        }

        private class Actual
        {
            public int Value { get; set; }
        }
    }
}