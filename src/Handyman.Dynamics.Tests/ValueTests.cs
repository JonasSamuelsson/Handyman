using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Handyman.Dynamics.Tests
{
    public class ValueTests
    {
        [Fact]
        public void GetValue()
        {
            var d = new DObject();

            Should.Throw<KeyNotFoundException>(() => d.GetValue<int>("key"));

            d.Set("key", 1);

            d.GetValue<int>("key").ShouldBe(1);
        }

        [Fact]
        public void GetValueConverted()
        {
            new DObject { ["key"] = "1" }.GetValue<int>("key").ShouldBe(1);
        }

        [Fact]
        public void GetValues()
        {
            var d = new DObject();

            Should.Throw<KeyNotFoundException>(() => d.GetValues<int>("key"));

            d.Set("key", 1);

            Should.Throw<InvalidCastException>(() => d.GetValues<int>("key"));

            d.Set("key", new[] { 1 });

            var values = d.GetValues<int>("key");

            values.ShouldBe(new[] { 1 });

            values.Add(2);

            d.GetValues<int>("key").ShouldBe(new[] { 1, 2 });
        }

        [Fact]
        public void GetValuesConverted()
        {
            var d = new DObject();

            Should.Throw<KeyNotFoundException>(() => d.GetValues<int>("key"));

            d.Set("key", "1");

            Should.Throw<InvalidCastException>(() => d.GetValues<int>("key"));

            d.Set("key", new[] { "1" });

            d.GetValues<int>("key").ShouldBe(new[] { 1 });
        }
    }
}