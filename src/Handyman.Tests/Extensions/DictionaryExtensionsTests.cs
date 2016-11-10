using System.Collections.Generic;
using Handyman.Extensions;
using Shouldly;

namespace Handyman.Tests.Extensions
{
    public class DictionaryExtensionsTests
    {
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

        public void GetOrAdd()
        {
            var dictionary = new Dictionary<int, string> { { 0, "Zero" } };

            dictionary.GetOrAdd(0, "0").ShouldBe("Zero");
            dictionary.GetOrAdd(1, "One").ShouldBe("One");
            dictionary.GetOrAdd(2, () => "Two").ShouldBe("Two");
            dictionary.GetOrAdd(3, i => i.ToString()).ShouldBe("3");
        }
    }
}