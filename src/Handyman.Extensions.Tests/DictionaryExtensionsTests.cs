using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Handyman.Extensions.Tests
{
    public class DictionaryExtensionsTests
    {
        [Fact]
        public void GetValueOrDefault()
        {
            var dictionary = new Dictionary<int, string>();

            dictionary.GetValueOrDefault(0).ShouldBe(null);
            dictionary.GetValueOrDefault(0, "default").ShouldBe("default");
            dictionary.GetValueOrDefault(0, () => "default").ShouldBe("default");

            dictionary.Add(0, "zero");

            dictionary.GetValueOrDefault(0).ShouldBe("zero");
            dictionary.GetValueOrDefault(0, "default").ShouldBe("zero");
            dictionary.GetValueOrDefault(0, () => "default").ShouldBe("zero");
        }

        [Fact]
        public void GetOrAdd()
        {
            var dictionary = new Dictionary<int, string> { { 0, "Zero" } };

            dictionary.GetOrAdd(0, "0").ShouldBe("Zero");
            dictionary.GetOrAdd(1, "One").ShouldBe("One");
            dictionary.GetOrAdd(2, () => "Two").ShouldBe("Two");
            dictionary.GetOrAdd(3, i => i.ToString()).ShouldBe("3");
        }

        [Fact]
        public void TryAdd()
        {
            var dictionary = new Dictionary<int, string>();

            dictionary.TryAdd(0, "zero").ShouldBe(true);

            dictionary[0].ShouldBe("zero");

            dictionary.TryAdd(0, "0").ShouldBe(false);

            dictionary[0].ShouldBe("zero");
        }
    }
}