using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Shouldly;
using Xunit;

namespace Handyman.DuckTyping.Tests
{
    public class DuckTypedListTests
    {
        [Fact]
        public void ShouldJustWork()
        {
            var dictionaries = new List<IDictionary<string,object>>();

            var dto1s = new DuckTypedList<Dto1>(dictionaries)
            {
                new Dto1 {Text = "success"}
            };

            var dto2s = new DuckTypedList<Dto2>(dictionaries);

            dto2s.Single().Text.ShouldBe("success");

            dto2s.Add(new Dto2 { Text = "double success" });

            dto1s.Last().Text.ShouldBe("double success");
        }

        private class Dto1 : DuckTypedObject
        {
            public Dto1() : base() { }
            public Dto1(ExpandoObject expando) : base(expando) { }

            public string Text
            {
                get => Get<string>(nameof(Text));
                set => Set(nameof(Text), value);
            }
        }

        private class Dto2 : DuckTypedObject
        {
            public Dto2() : base() { }
            public Dto2(ExpandoObject expando) : base(expando) { }

            public string Text
            {
                get => Get<string>(nameof(Text));
                set => Set(nameof(Text), value);
            }
        }
    }
}