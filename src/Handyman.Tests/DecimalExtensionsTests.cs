using System;
using Shouldly;

namespace Handyman.Tests
{
    public class DecimalExtensionsTests
    {
        public void ShouldCheckIfNumberIsBetween()
        {
            ((Decimal)0).IsBetween(1, 3).ShouldBe(false);
            ((Decimal)1).IsBetween(1, 3).ShouldBe(false);
            ((Decimal)2).IsBetween(1, 3).ShouldBe(true);
            ((Decimal)3).IsBetween(1, 3).ShouldBe(false);
            ((Decimal)4).IsBetween(1, 3).ShouldBe(false);
        }

        public void ShouldCheckIfNumberIsInRange()
        {
            ((Decimal)0).IsInRange(1, 3).ShouldBe(false);
            ((Decimal)1).IsInRange(1, 3).ShouldBe(true);
            ((Decimal)2).IsInRange(1, 3).ShouldBe(true);
            ((Decimal)3).IsInRange(1, 3).ShouldBe(true);
            ((Decimal)4).IsInRange(1, 3).ShouldBe(false);
        } 
    }
}