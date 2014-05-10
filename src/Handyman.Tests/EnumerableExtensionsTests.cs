using NUnit.Framework;
using Shouldly;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tests
{
    public class EnumerableExtensionsTests
    {
        public void ForEachYieldShouldExecuteProvidedActionForEachItemWhenTheReturnedEnumerableIsTraversed()
        {
            var sum = 0;
            var ints = new[] { 1, 2, 3 };
            ints.ForEachYield(i => sum += i);
            sum.ShouldBe(0);
            ints.ForEachYield(i => sum += i).ToList();
            sum.ShouldBe(6);
        }

        public void ForEachShouldExecuteProvidedActionForEachItem()
        {
            var sum = 0;
            new[] { 1, 2, 3 }.ForEach(i => sum += i);
            sum.ShouldBe(6);
        }

        public void IsEmptyShouldDetermineIfEnumerableIsEmpty()
        {
            var list = new List<int>();
            list.IsEmpty().ShouldBe(true);
            list.Add(0);
            list.IsEmpty().ShouldBe(false);
        }

        public void ShouldAppend()
        {
            new[] { 1, 2, 3 }.Append(4, 5, 6).ShouldBe(new[] { 1, 2, 3, 4, 5, 6 });
            new[] { 1, 2, 3 }.Append(new List<int> { 4, 5, 6 }).ShouldBe(new[] { 1, 2, 3, 4, 5, 6 });
        }

        public void ShouldPrepend()
        {
            new[] { 1, 2, 3 }.Prepend(4, 5, 6).ShouldBe(new[] { 4, 5, 6, 1, 2, 3 });
            new[] { 1, 2, 3 }.Prepend(new List<int> { 4, 5, 6 }).ShouldBe(new[] { 4, 5, 6, 1, 2, 3 });
        }
    }
}