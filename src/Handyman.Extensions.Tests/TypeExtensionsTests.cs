using System;
using System.Collections.Generic;
using System.Linq;
using Shouldly;
using Xunit;

namespace Handyman.Extensions.Tests
{
    public class TypeExtensionsTests
    {
        [Fact]
        public void ShouldCheckIfTypeIsConcreteClass()
        {
            typeof(Abstract).IsConcreteClass().ShouldBe(false);
            typeof(Concrete).IsConcreteClass().ShouldBe(true);
        }

        [Fact]
        public void ShouldCheckIfTypeIsConcreteClosedClass()
        {
            typeof(Concrete<>).IsConcreteClosedClass().ShouldBe(false);
            typeof(Concrete<string>).IsConcreteClosedClass().ShouldBe(true);
        }

        [Fact]
        public void ShouldCheckIfTypeIsConcreteClassClosing()
        {
            typeof(Concrete<>).IsConcreteClassClosing(typeof(Concrete<>)).ShouldBe(false);
            typeof(Concrete<object>).IsConcreteClassClosing(typeof(Concrete<>)).ShouldBe(false);
            typeof(ConcreteOfObject).IsConcreteClassClosing(typeof(Concrete<>)).ShouldBe(true);

            IReadOnlyCollection<Type> genericTypes;

            typeof(Concrete<>).IsConcreteClassClosing(typeof(Concrete<>), out genericTypes).ShouldBe(false);
            genericTypes.ShouldBe(null);

            typeof(Concrete<object>).IsConcreteClassClosing(typeof(Concrete<>), out genericTypes).ShouldBe(false);
            genericTypes.ShouldBe(null);

            typeof(ConcreteOfObject).IsConcreteClassClosing(typeof(Concrete<>), out genericTypes).ShouldBe(true);
            genericTypes.ShouldBe(new[] { typeof(Concrete<object>) });

            typeof(ClassOfIntAndString).IsConcreteClassClosing(typeof(IInterface<>)).ShouldBe(true);

            typeof(ClassOfIntAndString).IsConcreteClassClosing(typeof(IInterface<>), out genericTypes).ShouldBe(true);
            genericTypes.Count.ShouldBe(2);
            genericTypes.ShouldContain(typeof(IInterface<int>));
            genericTypes.ShouldContain(typeof(IInterface<string>));
        }

        [Fact]
        public void ShouldCheckIfTypeIsOfType()
        {
            typeof(object).IsOfType(typeof(object)).ShouldBe(true);
            typeof(object).IsOfType<object>().ShouldBe(true);

            typeof(object).IsOfType(typeof(IInterface)).ShouldBe(false);
            typeof(object).IsOfType<IInterface>().ShouldBe(false);

            typeof(IInterface).IsOfType(typeof(object)).ShouldBe(true);
            typeof(IInterface).IsOfType<object>().ShouldBe(true);

            typeof(IInterface<>).IsOfType(typeof(IInterface)).ShouldBe(true);
            typeof(IInterface<>).IsOfType<IInterface>().ShouldBe(true);

            typeof(IInterface<object>).IsOfType(typeof(IInterface<>)).ShouldBe(true);
            //typeof(IInterface<object>).IsOfType<IInterface<>>().ShouldBe(true);

            typeof(Abstract).IsOfType(typeof(IInterface)).ShouldBe(true);
            typeof(Abstract).IsOfType<IInterface>().ShouldBe(true);

            typeof(Abstract<>).IsOfType(typeof(IInterface<>)).ShouldBe(true);
            //typeof(Abstract<>).IsOfType<IInterface<>>().ShouldBe(true);

            typeof(Abstract<object>).IsOfType(typeof(IInterface<object>)).ShouldBe(true);
            typeof(Abstract<object>).IsOfType<IInterface<object>>().ShouldBe(true);

            typeof(Concrete).IsOfType(typeof(Abstract)).ShouldBe(true);
            typeof(Concrete).IsOfType<Abstract>().ShouldBe(true);

            typeof(Concrete<>).IsOfType(typeof(Abstract<>)).ShouldBe(true);
            //typeof(Concrete<>).IsOfType<Abstract<>>().ShouldBe(true);

            typeof(Concrete<object>).IsOfType(typeof(Concrete<>)).ShouldBe(true);
            //typeof(Concrete<object>).IsOfType<Concrete<>>().ShouldBe(true);

            typeof(ConcreteOfObject).IsOfType(typeof(Concrete<object>)).ShouldBe(true);
            typeof(ConcreteOfObject).IsOfType<Concrete<object>>().ShouldBe(true);

            typeof(ConcreteOfObject).IsOfType(typeof(IInterface)).ShouldBe(true);
            typeof(ConcreteOfObject).IsOfType<IInterface>().ShouldBe(true);

            typeof(IEnumerable<string>).IsOfType(typeof(IEnumerable<object>)).ShouldBe(true);
            typeof(IEnumerable<string>).IsOfType<IEnumerable<object>>().ShouldBe(true);
        }

        [Fact]
        public void ShouldCheckIfTypeIsSubType()
        {
            typeof(object).IsSubTypeOf(typeof(object)).ShouldBe(false);
            typeof(object).IsSubTypeOf<object>().ShouldBe(false);

            typeof(object).IsSubTypeOf(typeof(IInterface)).ShouldBe(false);
            typeof(object).IsSubTypeOf<IInterface>().ShouldBe(false);

            typeof(IInterface).IsSubTypeOf(typeof(object)).ShouldBe(true);
            typeof(IInterface).IsSubTypeOf<object>().ShouldBe(true);

            typeof(IInterface<>).IsSubTypeOf(typeof(IInterface)).ShouldBe(true);
            typeof(IInterface<>).IsSubTypeOf<IInterface>().ShouldBe(true);

            typeof(IInterface<object>).IsSubTypeOf(typeof(IInterface<>)).ShouldBe(true);
            //typeof(IInterface<object>).IsSubTypeOf<IInterface<>>().ShouldBe(true);

            typeof(Abstract).IsSubTypeOf(typeof(IInterface)).ShouldBe(true);
            typeof(Abstract).IsSubTypeOf<IInterface>().ShouldBe(true);

            typeof(Abstract<>).IsSubTypeOf(typeof(IInterface<>)).ShouldBe(true);
            //typeof(Abstract<>).IsSubTypeOf<IInterface<>>().ShouldBe(true);

            typeof(Abstract<object>).IsSubTypeOf(typeof(IInterface<object>)).ShouldBe(true);
            typeof(Abstract<object>).IsSubTypeOf<IInterface<object>>().ShouldBe(true);

            typeof(Concrete).IsSubTypeOf(typeof(Abstract)).ShouldBe(true);
            typeof(Concrete).IsSubTypeOf<Abstract>().ShouldBe(true);

            typeof(Concrete<>).IsSubTypeOf(typeof(Abstract<>)).ShouldBe(true);
            //typeof(Concrete<>).IsSubTypeOf<Abstract<>>().ShouldBe(true);

            typeof(Concrete<object>).IsSubTypeOf(typeof(Concrete<>)).ShouldBe(true);
            //typeof(Concrete<object>).IsSubTypeOf<Concrete<>>().ShouldBe(true);

            typeof(ConcreteOfObject).IsSubTypeOf(typeof(Concrete<object>)).ShouldBe(true);
            typeof(ConcreteOfObject).IsSubTypeOf<Concrete<object>>().ShouldBe(true);

            typeof(ConcreteOfObject).IsSubTypeOf(typeof(IInterface)).ShouldBe(true);
            typeof(ConcreteOfObject).IsSubTypeOf<IInterface>().ShouldBe(true);

            typeof(IEnumerable<string>).IsSubTypeOf(typeof(IEnumerable<object>)).ShouldBe(true);
            typeof(IEnumerable<string>).IsSubTypeOf<IEnumerable<object>>().ShouldBe(true);
        }

        [Fact]
        public void ShouldCheckIfTypeIsSuperType()
        {
            typeof(object).IsSuperTypeOf(typeof(object)).ShouldBe(false);
            typeof(object).IsSuperTypeOf<object>().ShouldBe(false);

            typeof(IInterface).IsSuperTypeOf(typeof(object)).ShouldBe(false);
            typeof(IInterface).IsSuperTypeOf<object>().ShouldBe(false);

            typeof(object).IsSuperTypeOf(typeof(IInterface)).ShouldBe(true);
            typeof(object).IsSuperTypeOf<IInterface>().ShouldBe(true);

            typeof(IInterface).IsSuperTypeOf(typeof(IInterface<>)).ShouldBe(true);
            //typeof(IInterface).IsSuperTypeOf<IInterface<>>().ShouldBe(true);

            typeof(IInterface<>).IsSuperTypeOf(typeof(IInterface<object>)).ShouldBe(true);
            typeof(IInterface<>).IsSuperTypeOf<IInterface<object>>().ShouldBe(true);

            typeof(IInterface).IsSuperTypeOf(typeof(Abstract)).ShouldBe(true);
            typeof(IInterface).IsSuperTypeOf<Abstract>().ShouldBe(true);

            typeof(IInterface<>).IsSuperTypeOf(typeof(Abstract<>)).ShouldBe(true);
            //typeof(IInterface<>).IsSuperTypeOf<Abstract<>>().ShouldBe(true);

            typeof(IInterface<object>).IsSuperTypeOf(typeof(Abstract<object>)).ShouldBe(true);
            typeof(IInterface<object>).IsSuperTypeOf<Abstract<object>>().ShouldBe(true);

            typeof(Abstract).IsSuperTypeOf(typeof(Concrete)).ShouldBe(true);
            typeof(Abstract).IsSuperTypeOf<Concrete>().ShouldBe(true);

            typeof(Abstract<>).IsSuperTypeOf(typeof(Concrete<>)).ShouldBe(true);
            //typeof(Abstract<>).IsSuperTypeOf<Concrete<>>().ShouldBe(true);

            typeof(Concrete<>).IsSuperTypeOf(typeof(Concrete<object>)).ShouldBe(true);
            typeof(Concrete<>).IsSuperTypeOf<Concrete<object>>().ShouldBe(true);

            typeof(Concrete<object>).IsSuperTypeOf(typeof(ConcreteOfObject)).ShouldBe(true);
            typeof(Concrete<object>).IsSuperTypeOf<ConcreteOfObject>().ShouldBe(true);

            typeof(IInterface).IsSuperTypeOf(typeof(ConcreteOfObject)).ShouldBe(true);
            typeof(IInterface).IsSuperTypeOf<ConcreteOfObject>().ShouldBe(true);

            typeof(IEnumerable<object>).IsSuperTypeOf(typeof(IEnumerable<string>)).ShouldBe(true);
            typeof(IEnumerable<object>).IsSuperTypeOf<IEnumerable<string>>().ShouldBe(true);
        }

        [Fact]
        public void ShouldGetSuperTypes()
        {
            var expextedTypes = new[]
            {
                typeof (object),
                typeof (IInterface),
                typeof (IInterface<>),
                typeof (IInterface<object>),
                typeof (Abstract),
                typeof (Abstract<>),
                typeof (Abstract<object>),
                typeof (Concrete<>),
                typeof (Concrete<object>),
            }.OrderBy(x => x.FullName, StringComparer.Ordinal);
            typeof(ConcreteOfObject).GetSuperTypes().OrderBy(x => x.FullName, StringComparer.Ordinal).ShouldBe(expextedTypes);
        }

        [Fact]
        public void ShouldCheckIsConcreteSubClassOf()
        {
            typeof(Abstract).IsConcreteSubClassOf(typeof(IInterface)).ShouldBe(false);
            typeof(Abstract).IsConcreteSubClassOf<IInterface>().ShouldBe(false);

            typeof(Concrete).IsConcreteSubClassOf(typeof(IInterface)).ShouldBe(true);
            typeof(Concrete).IsConcreteSubClassOf<IInterface>().ShouldBe(true);
        }

        private interface IInterface { }
        private interface IInterface<T> : IInterface { }
        private abstract class Abstract : IInterface { }
        private abstract class Abstract<T> : Abstract, IInterface<T> { }
        private class Concrete : Abstract { }
        private class Concrete<T> : Abstract<T> { }
        private class ConcreteOfObject : Concrete<object> { }
        private class ClassOfIntAndString : IInterface<int>, IInterface<string> { }
    }
}