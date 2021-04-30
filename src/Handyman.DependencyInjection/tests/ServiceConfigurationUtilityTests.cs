using Handyman.DependencyInjection.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;
using Xunit;

namespace Handyman.DependencyInjection.Tests
{
    public class ServiceConfigurationUtilityTests
    {
        [Fact]
        public void ShouldAdd()
        {
            var services = new ServiceCollection();

            var serviceType = typeof(IService);
            var implementationType1 = typeof(Implementation1);
            var implementationType2 = typeof(Implementation2);
            var lifetime = ServiceLifetime.Transient;
            var policy = ServiceConfigurationPolicy.Add;

            ServiceConfigurationUtility.Configure(services, serviceType, implementationType1, lifetime, policy);
            ServiceConfigurationUtility.Configure(services, serviceType, implementationType1, lifetime, policy);
            ServiceConfigurationUtility.Configure(services, serviceType, implementationType2, lifetime, policy);
            ServiceConfigurationUtility.Configure(services, serviceType, implementationType2, lifetime, policy);

            services.Count.ShouldBe(4);
            services.Count(x => x.Lifetime == lifetime && x.ServiceType == serviceType).ShouldBe(4);
            services.Count(x => x.ImplementationType == implementationType1).ShouldBe(2);
            services.Count(x => x.ImplementationType == implementationType2).ShouldBe(2);
        }

        [Fact]
        public void ShouldTryAdd()
        {
            var services = new ServiceCollection();

            var serviceType = typeof(IService);
            var implementationType1 = typeof(Implementation1);
            var implementationType2 = typeof(Implementation2);
            var lifetime = ServiceLifetime.Transient;
            var policy = ServiceConfigurationPolicy.TryAdd;

            ServiceConfigurationUtility.Configure(services, serviceType, implementationType1, lifetime, policy);
            ServiceConfigurationUtility.Configure(services, serviceType, implementationType1, lifetime, policy);
            ServiceConfigurationUtility.Configure(services, serviceType, implementationType2, lifetime, policy);
            ServiceConfigurationUtility.Configure(services, serviceType, implementationType2, lifetime, policy);

            services.Count.ShouldBe(1);
            services.Count(x => x.Lifetime == lifetime && x.ServiceType == serviceType).ShouldBe(1);
            services.Count(x => x.ImplementationType == implementationType1).ShouldBe(1);
        }

        [Fact]
        public void ShouldTryAddEnumerable()
        {
            var services = new ServiceCollection();

            var serviceType = typeof(IService);
            var implementationType1 = typeof(Implementation1);
            var implementationType2 = typeof(Implementation2);
            var lifetime = ServiceLifetime.Transient;
            var policy = ServiceConfigurationPolicy.TryAddEnumerable;

            ServiceConfigurationUtility.Configure(services, serviceType, implementationType1, lifetime, policy);
            ServiceConfigurationUtility.Configure(services, serviceType, implementationType1, lifetime, policy);
            ServiceConfigurationUtility.Configure(services, serviceType, implementationType2, lifetime, policy);
            ServiceConfigurationUtility.Configure(services, serviceType, implementationType2, lifetime, policy);

            services.Count.ShouldBe(2);
            services.Count(x => x.Lifetime == lifetime && x.ServiceType == serviceType).ShouldBe(2);
            services.Count(x => x.ImplementationType == implementationType1).ShouldBe(1);
            services.Count(x => x.ImplementationType == implementationType2).ShouldBe(1);
        }

        private class IService { }

        private class Implementation1 : IService { }

        private class Implementation2 : IService { }
    }
}