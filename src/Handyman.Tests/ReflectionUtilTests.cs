using Shouldly;

namespace Handyman.Tests
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

        public void ShouldGetPropertyValue()
        {
            var instance = new Parent { Child = { Text = "foo" } };
            ReflectionUtil.GetPropertyValue(instance, "Child").ShouldBe(Child.Instance);
            ReflectionUtil.GetPropertyValue<Child>(instance, "Child").ShouldBe(Child.Instance);
            ReflectionUtil.GetPropertyValue(instance, "Child", "Text").ShouldBe("foo");
            ReflectionUtil.GetPropertyValue<string>(instance, "Child", "Text").ShouldBe("foo");
            ReflectionUtil.GetPropertyValue(instance, new[] { "Child", "Text", "Length" }).ShouldBe(3);
            ReflectionUtil.GetPropertyValue<int>(instance, new[] { "Child", "Text", "Length" }).ShouldBe(3);
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