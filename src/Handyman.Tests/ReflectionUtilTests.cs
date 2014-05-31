using Shouldly;

namespace Handyman.Tests
{
    public class ReflectionUtilTests
    {
        public void GetPropertyNameShouldGetTheNameOfTheProperty()
        {
            ReflectionUtil.GetPropertyName(() => new TestClass().Text).ShouldBe("Text");
            ReflectionUtil.GetPropertyName<TestClass>(x => x.Text).ShouldBe("Text");
        }

        public void ShouldGetPropertyNames()
        {
            var result = new[] { "Text", "Length" };
            ReflectionUtil.GetPropertyNames(() => new TestClass().Text.Length).ShouldBe(result);
            ReflectionUtil.GetPropertyNames<TestClass>(x => x.Text.Length).ShouldBe(result);
        }

        private class TestClass
        {
            public string Text { get; set; }
        }
    }
}