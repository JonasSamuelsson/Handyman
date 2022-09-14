using Shouldly;
using System;
using Xunit;

namespace Handyman.DataContractValidator.Tests
{
    public class DataContractValidatorOptionsTests
    {
        [Fact]
        public void ShouldHaveCorrectDefaultValues()
        {
            var options = new DataContractValidatorOptions();

            options.AllowPropertiesNotFoundInDataContract.ShouldBeFalse();
            options.EnumValueNameComparison.ShouldBe(StringComparison.Ordinal);
            options.PropertyNameComparison.ShouldBe(StringComparison.Ordinal);
        }
    }
}