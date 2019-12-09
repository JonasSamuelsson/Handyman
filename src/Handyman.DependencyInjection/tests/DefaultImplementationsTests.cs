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
}
