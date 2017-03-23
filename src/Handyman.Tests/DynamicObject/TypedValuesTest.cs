using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Handyman.Tests.DynamicObject
{
    public class TypedValuesTest
    {
        [Fact]
        public void Value()
        {
            var d = new Handyman.DynamicObject.DynamicObject();

            Should.Throw<Exception>(() => d.Value("key"));

            d.Set("key", "value");

            d.Value("key").ShouldBe("value");
        }

        [Fact]
        public void TypedValue()
        {
            var d = new Handyman.DynamicObject.DynamicObject();

            Should.Throw<KeyNotFoundException>(() => d.Value<int>("key"));

            d.Set("key", 1);

            d.Value<int>("key").ShouldBe(1);
        }

        [Fact]
        public void Convert()
        {
            var d = new Handyman.DynamicObject.DynamicObject();

            d.Set("key", "1");

            d.Value<int>("key").ShouldBe(1);
        }

        [Fact]
        public void Values()
        {
            var d = new Handyman.DynamicObject.DynamicObject();

            Should.Throw<KeyNotFoundException>(() => d.Value("key"));

            d.Set("key", "value");

            Should.Throw<InvalidCastException>(() => d.Values("key"));

            d.Set("key", new[] { "value1" });

            var values = d.Values("key");

            values.ShouldBe(new[] { "value1" });

            values.Add("value2");

            d.Values("key").ShouldBe(new[] { "value1", "value2" });
        }

        [Fact]
        public void TypedValues()
        {
            var d = new Handyman.DynamicObject.DynamicObject();

            Should.Throw<KeyNotFoundException>(() => d.Value("key"));

            d.Set("key", 1);

            Should.Throw<InvalidCastException>(() => d.Values("key"));

            d.Set("key", new object[] { 1, "2" });

            var values = d.Values<int>("key");

            values.ShouldBe(new[] { 1, 2 });

            values.Add(3, "4");

            d.Values<int>("key").ShouldBe(new[] { 1, 2, 3, 4 });
        }
    }
}
