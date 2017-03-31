using System;
using System.Linq;
using Handyman.Dynamics;
using Shouldly;
using Xunit;

namespace Handyman.Tests.Dynamics
{
    public class ListTests
    {
        [Fact]
        public void Add()
        {
            var list1 = new DList<DObject>();

            list1.Add(new { value = "1" });

            list1.Single().GetString("value").ShouldBe("1");

            Should.Throw<InvalidCastException>(() => list1.Add(2));

            var list2 = new DList<int>();

            list2.Add(1);

            list2.Single().ShouldBe(1);

            Should.Throw<InvalidCastException>(() => list2.Add(new { value = "2" }));
        }

        [Fact]
        public void Clear()
        {
            var list = new DList<int> { 1 };

            list.Clear();

            list.Any().ShouldBeFalse();
        }

        [Fact]
        public void Remove()
        {
            var list = new DList<int> { 0, 1, 2 };

            list.Remove(item => item == 1).ShouldBe(1);

            list.ShouldBe(new[] { 0, 2 });

            list.Remove(1);

            list.ShouldBe(new[] { 0 });
        }

        [Fact]
        public void Insert()
        {
            var list = new DList<int> { 1, 3 };

            list.Insert(1, 2);

            list.ShouldBe(new[] { 1, 2, 3 });
        }
    }
}
