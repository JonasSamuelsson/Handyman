using System;
using System.Collections.Generic;
using System.Linq;
using Handyman.Dynamics;
using Newtonsoft.Json;
using Shouldly;
using Xunit;

namespace Handyman.Tests.Dynamics
{
    public class ObjectTests
    {
        [Fact]
        public void CreateFromDictionary()
        {
            DObject.Create(new Dictionary<string, object> { { "key", "value" } })
                .Contains("key");
        }

        [Fact]
        public void CreateFromObject()
        {
            DObject.Create(new { key = "value" })
                .Contains("key");
        }

        [Fact]
        public void CreateFromUnsupportedType()
        {
            Should.Throw<InvalidCastException>(() => DObject.Create(null));
            Should.Throw<InvalidCastException>(() => DObject.Create(""));
            Should.Throw<InvalidCastException>(() => DObject.Create(0));
            Should.Throw<InvalidCastException>(() => DObject.Create(new object[] { }));
        }

        [Fact]
        public void GetObject()
        {
            DObject.Create(new { @object = new { @string = "1" } })
                 .GetObject("object").GetString("string").ShouldBe("1");
        }

        [Fact]
        public void GetObjects()
        {
            var source = new
            {
                items = new[]
               {
                 new {value = "1"},
                 new {value = "2"},
                 new {value = "3"}
              }
            };

            var objects = DObject.Create(source).GetObjects("items");

            objects.Count().ShouldBe(3);
            objects.ElementAt(0).GetString("value").ShouldBe("1");
            objects.ElementAt(1).GetString("value").ShouldBe("2");
            objects.ElementAt(2).GetString("value").ShouldBe("3");
        }

        [Fact]
        public void Serialization()
        {
            var source = new { children = new[] { new { child = new { value = 1 } } } };

            var d1 = DObject.Create(source);
            var json = JsonConvert.SerializeObject(d1);
            var d2 = JsonConvert.DeserializeObject<DObject>(json);

            d1.GetObjects("children").Single().GetObject("child").GetString("value").ShouldBe("1");
            d2.GetObjects("children").Single().GetObject("child").GetString("value").ShouldBe("1");
        }
    }
}