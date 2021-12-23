using System.Globalization;

namespace Handyman.Extensions.Tests
{
    public class ConfigurationTests
    {
        [Fact]
        public void FormatProviderShouldBeCurrentCulture()
        {
            Configuration.Reset();
            Configuration.FormatProvider().ShouldBe(CultureInfo.CurrentCulture);
        }

        [Fact]
        public void StringComparisonShouldBeCurrentCulture()
        {
            Configuration.Reset();
            Configuration.StringComparison().ShouldBe(StringComparison.CurrentCulture);
        }
    }
}