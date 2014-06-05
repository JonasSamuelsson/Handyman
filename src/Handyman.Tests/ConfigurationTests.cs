using Shouldly;
using System;
using System.Globalization;

namespace Handyman.Tests
{
    public class ConfigurationTests
    {
        public void FormatProviderShouldBeCurrentCulture()
        {
            Configuration.Reset();
            Configuration.FormatProvider().ShouldBe(CultureInfo.CurrentCulture);
        }

        public void StringComparisonShouldBeCurrentCulture()
        {
            Configuration.Reset();
            Configuration.StringComparison().ShouldBe(StringComparison.CurrentCulture);
        }
    }
}