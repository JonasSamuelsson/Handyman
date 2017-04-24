using Handyman.Dynamics;
using Shouldly;
using System;
using System.Collections.Generic;
using Xunit;

namespace Handyman.Tests.Dynamics
{
    public class StringTests
    {
        [Fact]
        public void GetString()
        {
            var d = new DObject();

            Should.Throw<KeyNotFoundException>(() => d.GetString("key"));

            d.Set("key", "value");

            d.GetString("key").ShouldBe("value");
        }

        [Fact]
        public void GetStringConverted()
        {
            new DObject { ["key"] = 1 }.GetString("key").ShouldBe("1");
        }

        [Fact]
        public void GetStrings()
        {
            var d = new DObject();

            Should.Throw<KeyNotFoundException>(() => d.GetStrings("key"));

            d.Set("key", "value");

            Should.Throw<InvalidCastException>(() => d.GetStrings("key"));

            d.Set("key", new[] { "value1" });

            var values = d.GetStrings("key");

            values.ShouldBe(new[] { "value1" });

            values.Add("value2");

            d.GetStrings("key").ShouldBe(new[] { "value1", "value2" });
        }

        [Fact]
        public void GetStringsConverted()
        {
            var d = new DObject();

            Should.Throw<KeyNotFoundException>(() => d.GetStrings("key"));

            d.Set("key", 1);

            Should.Throw<InvalidCastException>(() => d.GetStrings("key"));

            d.Set("key", new[] { 1 });

            d.GetStrings("key").ShouldBe(new[] { "1" });
        }

        [Fact]
        public void GetStringOrDefault()
        {
            var d = new DObject { ["foo"] = "bar" };

            d.GetStringOrDefault("foo", "default").ShouldBe("bar");
            d.GetStringOrDefault("bar", "default").ShouldBe("default");
        }
    }
}
