using Handyman.Converters;
using Shouldly;
using System;

namespace Handyman.Tests.Converters
{
    public class HasValueConverterTests
    {
        public void ShouldConvertNullToFalse()
        {
            new HasValueConverter().Convert(null, null, null, null).ShouldBe(false);
        }

        public void ShouldConvertValueToTrue()
        {
            new HasValueConverter().Convert(0, null, null, null).ShouldBe(true);
        }

        public void ShouldConvertReferenceToTrue()
        {
            new HasValueConverter().Convert(new object(), null, null, null).ShouldBe(true);
        }

        public void ConvertBackShouldNotBeImplemented()
        {
            Should.Throw<NotImplementedException>(() => new HasValueConverter().ConvertBack(false, null, null, null));
        }
    }
}