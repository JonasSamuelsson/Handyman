using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;
using Xunit;

namespace Handyman.DependencyInjection.Tests
{
    public class IgnoreServiceAttributeTests
    {
        [Fact]
        public void ShouldNotScanTypesMarkedWithAttribute()
        {
            var services = new ServiceCollection();

            services.Scan(x => x.AssemblyContainingTypeOf(this).ConfigureConcreteClassesOf<IInterface>());

            var serviceProvider = services.BuildServiceProvider();

            var instances = serviceProvider.GetServices<IInterface>();

            instances.Single().ShouldBeOfType<ClassWithoutAttribute>();
        }

        [Fact]
        public void ShouldAcceptExplicitlyRegisteredServicesWithAttribute()
        {
            var services = new ServiceCollection();

            services.AddTransient<IInterface, ClassWithAttribute>();

            var serviceProvider = services.BuildServiceProvider();

            serviceProvider.GetService<IInterface>().ShouldBeOfType<ClassWithAttribute>();
        }

        private interface IInterface { }

        private class ClassWithoutAttribute : IInterface { }

        [IgnoreService]
        private class ClassWithAttribute : IInterface { }
    }
}