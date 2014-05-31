using Handyman.Converters;
using Shouldly;
using System.Windows;

namespace Handyman.Tests.Converters
{
    public class Bool2VisibilityConverterTests
    {
        public void ShouldConvert()
        {
            new Bool2VisibilityConverter().Convert(false, null, null, null).ShouldBe(Visibility.Collapsed);
            new Bool2VisibilityConverter().Convert(true, null, null, null).ShouldBe(Visibility.Visible);

            new Bool2VisibilityConverter().ConvertBack(Visibility.Collapsed, null, null, null).ShouldBe(false);
            new Bool2VisibilityConverter().ConvertBack(Visibility.Visible, null, null, null).ShouldBe(true);

            new Bool2VisibilityConverter { False = Visibility.Visible }.Convert(false, null, null, null).ShouldBe(Visibility.Visible);
            new Bool2VisibilityConverter { True = Visibility.Hidden }.Convert(true, null, null, null).ShouldBe(Visibility.Hidden);
        }
    }
}