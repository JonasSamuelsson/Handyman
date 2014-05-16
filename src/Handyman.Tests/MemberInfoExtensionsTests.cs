using System;
using System.Linq;
using Shouldly;

namespace Handyman.Tests
{
    public class MemberInfoExtensionsTests
    {
        public void ShouldCheckIfMemberHasAttribute()
        {
            typeof(SuperType).Has<TestAttribute>().ShouldBe(true);
            typeof(SubType).Has<TestAttribute>().ShouldBe(false);
            typeof(SubType).Has<TestAttribute>(true).ShouldBe(true);
        }

        public void ShouldGetAttributes()
        {
            typeof(SuperType).Get<TestAttribute>().Any().ShouldBe(true);
            typeof(SubType).Get<TestAttribute>().Any().ShouldBe(false);
            typeof(SubType).Get<TestAttribute>(true).Any().ShouldBe(true);   
        }

        [TestAttribute]
        private class SuperType { }
        private class SubType : SuperType { }
        private class TestAttribute : Attribute { }
    }
}