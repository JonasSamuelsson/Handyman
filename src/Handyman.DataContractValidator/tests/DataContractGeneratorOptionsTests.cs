using Shouldly;
using Xunit;

namespace Handyman.DataContractValidator.Tests
{
    public class DataContractGeneratorOptionsTests
    {
        [Fact]
        public void ShouldHaveCorrectDefaultValues()
        {
            var options = new DataContractGeneratorOptions();

            options.Indentation.ShouldBe("   ");
            options.SortPropertiesAlphabetically.ShouldBeFalse();
        }
    }
}