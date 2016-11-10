using System;
using System.Globalization;
using Handyman.Extensions;
using Shouldly;
using Xunit;

namespace Handyman.Tests.Extensions
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