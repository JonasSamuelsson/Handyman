using Shouldly;
using System.Collections.Generic;
using Handyman.Mediator.Pipeline;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class ExtensionsTests
    {
        [Fact]
        public void ToListOptimizedShouldReturnTheSameInstance()
        {
            var ints = new List<int> { 1, 2, 3 };

            ints.ToListOptimized().ShouldBe(ints);
        }
    }
}
