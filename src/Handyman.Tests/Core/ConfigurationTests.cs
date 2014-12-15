using System;
using System.Globalization;
using Shouldly;

namespace Handyman.Tests.Core
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