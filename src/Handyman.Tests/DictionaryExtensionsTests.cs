using System.Collections.Generic;
using Shouldly;

namespace Handyman.Tests
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
    }
}