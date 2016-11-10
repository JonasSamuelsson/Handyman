using System;
using Handyman.Wpf.Converters;
using Shouldly;
using Xunit;

namespace Handyman.Tests.Wpf.Converters
{
    public class HasValueConverterTests
    {
        [Fact]
        public void ShouldConvertNullToFalse()
        {
            new HasValueConverter().Convert(null, null, null, null).ShouldBe(false);
        }

        [Fact]
        public void ShouldConvertValueToTrue()
        {
            new HasValueConverter().Convert(0, null, null, null).ShouldBe(true);
        }

        [Fact]
        public void ShouldConvertReferenceToTrue()
        {
            new HasValueConverter().Convert(new object(), null, null, null).ShouldBe(true);
        }

        [Fact]
        public void ConvertBackShouldNotBeImplemented()
        {
            Should.Throw<NotImplementedException>(() => new HasValueConverter().ConvertBack(false, null, null, null));
        }
    }
}