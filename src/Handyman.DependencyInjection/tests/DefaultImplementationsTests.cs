using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Handyman.DependencyInjection.Tests
{
    public class DefaultImplementationsTests
    {
        [Fact]
        public void ShouldRegisterDefaultImplementations()
        {
            var services = new ServiceCollection();

            services.Scan(scanner => scanner.Types(GetType().GetNestedTypes(BindingFlags.NonPublic)).ConfigureDefaultImplementations());

            var descriptor = services.Single();

            descriptor.ImplementationType.ShouldBe(typeof(Foo));
            descriptor.Lifetime.ShouldBe(ServiceLifetime.Transient);
            descriptor.ServiceType.ShouldBe(typeof(IFoo));
        }

        private interface IFoo { }

        private class Foo : IFoo { }

        private interface IBar { }

        private class Bar { }
    }

    public class ClassesWithServiceAttributeTests
    {
        [Fact]
        public void ShouldConfigureClassesWithServiceAttribute()
        {
            var services = new ServiceCollection();

            services.Scan(scanner => scanner.Types(GetType().GetNestedTypes(BindingFlags.NonPublic)).ConfigureClassesWithServiceAttribute());

            services.Count.ShouldBe(3);

            services.ShouldContain(x => x.ServiceType == typeof(IFoo)
                                        && x.ImplementationType == typeof(Foo)
                                        && x.Lifetime == ServiceLifetime.Transient);

            services.ShouldContain(x => x.ServiceType == typeof(IFoo)
                                        && x.ImplementationType == typeof(FooBar)
                                        && x.Lifetime == ServiceLifetime.Scoped);

            services.ShouldContain(x => x.ServiceType == typeof(IBar)
                                        && x.ImplementationType == typeof(FooBar)
                                        && x.Lifetime == ServiceLifetime.Singleton);
        }

        private interface IFoo { }
        private interface IBar { }

        [Service(typeof(IFoo))]
        private class Foo : IFoo { }

        [Service(typeof(IFoo), ServiceLifetime.Scoped)]
        [Service(typeof(IBar), ServiceLifetime.Singleton)]
        private class FooBar : IFoo, IBar { }
    }
}
