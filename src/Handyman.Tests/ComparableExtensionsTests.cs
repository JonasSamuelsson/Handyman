using System;
using Shouldly;

namespace Handyman.Tests.Core
{
    public class ComparableExtensionsTests
    {
        public void ShouldCheckIfValueIsInRange()
        {
            1.IsInRange(2, 4).ShouldBe(false);
            2.IsInRange(2, 4).ShouldBe(true);
            3.IsInRange(2, 4).ShouldBe(true);
            4.IsInRange(2, 4).ShouldBe(true);
            5.IsInRange(2, 4).ShouldBe(false);

            1.1.IsInRange(1.2, 1.4).ShouldBe(false);
            1.2.IsInRange(1.2, 1.4).ShouldBe(true);
            1.3.IsInRange(1.2, 1.4).ShouldBe(true);
            1.4.IsInRange(1.2, 1.4).ShouldBe(true);
            1.5.IsInRange(1.2, 1.4).ShouldBe(false);
        }

        public void Clamp()
        {
            1.Clamp(2, 4).ShouldBe(2);
            2.Clamp(2, 4).ShouldBe(2);
            3.Clamp(2, 4).ShouldBe(3);
            4.Clamp(2, 4).ShouldBe(4);
            5.Clamp(2, 4).ShouldBe(4);

            1.Clamp(4, 2).ShouldBe(2);
            2.Clamp(4, 2).ShouldBe(2);
            3.Clamp(4, 2).ShouldBe(3);
            4.Clamp(4, 2).ShouldBe(4);
            5.Clamp(4, 2).ShouldBe(4);
        }

        public void ClampShouldThrowIfAnyParameterIsNull()
        {
            string @null = null;

            Should.Throw<ArgumentNullException>(() => @null.Clamp("", ""));
            Should.Throw<ArgumentNullException>(() => "".Clamp(@null, ""));
            Should.Throw<ArgumentNullException>(() => "".Clamp("", @null));
        }
    }
}