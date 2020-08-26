using Shouldly;
using System.Dynamic;
using System.Linq;
using Xunit;

namespace Handyman.DuckTyping.Tests
{
    public class DuckTypedObjectTests
    {
        [Fact]
        public void ShouldHandleValues()
        {
            var expando = new ExpandoObject();

            var dto1 = new Dto1(expando)
            {
                Value = 123
            };

            var dto2 = new Dto2(expando);

            dto2.Value.ShouldBe(123);

            dto2.Value = 321;

            dto1.Value.ShouldBe(321);
        }

        [Fact]
        public void ShouldHandleObjects()
        {
            var expando = new ExpandoObject();

            var dto1 = new Dto1(expando)
            {
                Object = new Dto1
                {
                    Value = 123
                }
            };

            var dto2 = new Dto2(expando);

            dto2.Object.Value.ShouldBe(123);

            dto2.Object.Value = 321;

            dto1.Object.Value.ShouldBe(321);
        }

        [Fact]
        public void ShouldHandleLists()
        {
            var expando = new ExpandoObject();

            var dto1 = new Dto1(expando)
            {
                List = new DuckTypedList<Dto1>
                {
                    new Dto1
                    {
                        Value = 123
                    }
                }
            };

            var dto2 = new Dto2(expando);

            dto2.List[0].Value.ShouldBe(123);

            dto2.List.Add(new Dto2 { Value = 321 });

            dto1.List.Sum(x => x.Value).ShouldBe(444);
        }

        [Fact]
        public void UninitializedPropertiesShouldReturnTheTypeDefaultValue()
        {
            var dto = new Dto1();

            dto.Object.ShouldBeNull();
            dto.Value.ShouldBe(0);
        }

        public class Dto1 : DuckTypedObject
        {
            public Dto1() { }
            public Dto1(ExpandoObject expando) : base(expando) { }

            public DuckTypedList<Dto1> List
            {
                get => Get<DuckTypedList<Dto1>>(nameof(List));
                set => Set(nameof(List), value);
            }

            public Dto1 Object
            {
                get => Get<Dto1>(nameof(Object));
                set => Set(nameof(Object), value);
            }

            public int Value
            {
                get => Get<int>(nameof(Value));
                set => Set(nameof(Value), value);
            }
        }

        public class Dto2 : DuckTypedObject
        {
            public Dto2() { }
            public Dto2(ExpandoObject expando) : base(expando) { }

            public DuckTypedList<Dto2> List
            {
                get => Get<DuckTypedList<Dto2>>(nameof(List));
                set => Set(nameof(List), value);
            }

            public Dto2 Object
            {
                get => Get<Dto2>(nameof(Object));
                set => Set(nameof(Object), value);
            }

            public int Value
            {
                get => Get<int>(nameof(Value));
                set => Set(nameof(Value), value);
            }
        }
    }
}
