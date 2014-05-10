using Shouldly;
using System;

namespace Handyman.Tests
{
    public class TypeExtensionsTests
    {
        public void ShouldCheckIfTypeIsConcreteClass()
        {
            typeof(Abstract).IsConcreteClass().ShouldBe(false);
            typeof(Concrete).IsConcreteClass().ShouldBe(true);
        }

        public void ShouldCheckIfTypeIsConcreteClosedClass()
        {
            typeof(Concrete<>).IsConcreteClosedClass().ShouldBe(false);
            typeof(Concrete<string>).IsConcreteClosedClass().ShouldBe(true);
        }

        public void ShouldCheckIfTypeIsConcreteClassClosing()
        {
            typeof(Concrete<>).IsConcreteClassClosing(typeof(Concrete<>)).ShouldBe(false);
            typeof(Concrete<string>).IsConcreteClassClosing(typeof(Concrete<>)).ShouldBe(true);

            Type genericType;
            typeof(ConcreteOfString).IsConcreteClassClosing(typeof(Concrete<>), out genericType).ShouldBe(true);
            genericType.ShouldBe(typeof(Concrete<string>));
        }

        //public void ShouldCheckIfTypeXyz() { }
        //public void ShouldCheckIfTypeXyz() { }
        //public void ShouldCheckIfTypeXyz() { }
        //public void ShouldCheckIfTypeXyz() { }
        //public void ShouldCheckIfTypeXyz() { }
        //public void ShouldCheckIfTypeXyz() { }
        //public void ShouldCheckIfTypeXyz() { }
        //public void ShouldCheckIfTypeXyz() { }
        //public void ShouldCheckIfTypeXyz() { }
        //public void ShouldCheckIfTypeXyz() { }

        private interface IInterface { }
        private interface IInterface<T> { }
        private abstract class Abstract : IInterface { }
        private abstract class Abstract<T> : Abstract, IInterface<T> { }
        private class Concrete : Abstract { }
        private class Concrete<T> : Abstract<T> { }
        private class ConcreteOfString : Concrete<string> { }
    }
}