using Shouldly;
using Xunit;

namespace Handyman.DataContractValidator.Tests
{
    public class DataContractValidatorTests
    {
        [Fact]
        public void ValidatorsShouldNotShareOptions()
        {
            new DataContractValidator().Options.ShouldNotBe(new DataContractValidator().Options);
        }
    }
}