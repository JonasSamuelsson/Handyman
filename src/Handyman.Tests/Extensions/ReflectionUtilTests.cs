using Handyman.Extensions;
using Shouldly;

namespace Handyman.Tests.Extensions
{
    public class ReflectionUtilTests
    {
        public void ShuoldGetPropertName()
        {
            ReflectionUtil.GetPropertyName(() => new Parent().Child).ShouldBe("Child");
            ReflectionUtil.GetPropertyName<Parent>(x => x.Child).ShouldBe("Child");
        }

        public void ShouldGetPropertyNames()
        {
            var result = new[] { "Child", "Text" };
            ReflectionUtil.GetPropertyNames(() => new Parent().Child.Text).ShouldBe(result);
            ReflectionUtil.GetPropertyNames<Parent>(x => x.Child.Text).ShouldBe(result);
        }

        public void ShouldGetProperty()
        {
            var instance = new Parent { Child = { Text = "foo" } };
            ReflectionUtil.GetProperty(instance, "Child").ShouldBe(Child.Instance);
            ReflectionUtil.GetProperty<Child>(instance, "Child").ShouldBe(Child.Instance);
            ReflectionUtil.GetProperty(instance, new[] { "Child", "Text", "Length" }).ShouldBe(3);
            ReflectionUtil.GetProperty<int>(instance, new[] { "Child", "Text", "Length" }).ShouldBe(3);
        }

        public void ShouldSetProperty()
        {
            var child = new Child();
            ReflectionUtil.SetProperty(child, "Text", "foo");
            child.Text.ShouldBe("foo");

            var parent = new Parent();
            ReflectionUtil.SetProperty(parent, new[] { "Child", "Text" }, "bar");
            parent.Child.Text.ShouldBe("bar");
        }

        private class Parent
        {
            public Child Child
            {
                get { return Child.Instance; }
            }
        }

        private class Child
        {
            public static readonly Child Instance = new Child();
            public string Text { get; set; }
        }
    }
}