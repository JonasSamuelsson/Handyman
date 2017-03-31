using System;
using System.Collections.Generic;
using Handyman.Dynamics;
using Shouldly;
using Xunit;

namespace Handyman.Tests.Dynamics
{
    public class BasicsTests
    {
        [Fact]
        public void Add()
        {
            var d = new DObject();

            d.Add("key", "value");

            Should.Throw<ArgumentException>(() => d.Add("key", "value"));
        }

        [Fact]
        public void Clear()
        {
            var d = new DObject { ["key"] = "value" };

            d.Clear();

            d.Contains("key").ShouldBeFalse();
        }

        [Fact]
        public void Contains()
        {
            var d = new DObject();

            d.Contains("key").ShouldBeFalse();

            d["key"] = "value";

            d.Contains("key").ShouldBeTrue();
        }

        [Fact]
        public void Remove()
        {
            var d = new DObject { ["key1"] = "value" };

            d.Rename("key1", "key2");

            d.Contains("key1").ShouldBeFalse();
            d.Contains("key2").ShouldBeTrue();

            Should.Throw<KeyNotFoundException>(() => d.Rename("key1", "key2"));
        }

        [Fact]
        public void Rename()
        {
            var d = new DObject { ["key"] = "value" };

            d.Remove("key");

            d.Contains("key").ShouldBeFalse();
        }
    }
}