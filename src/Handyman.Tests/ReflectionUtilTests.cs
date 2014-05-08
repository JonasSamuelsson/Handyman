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

        private class TestClass
        {
            public string Text { get; set; }
        }
    }
}