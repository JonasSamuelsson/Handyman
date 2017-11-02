using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Handyman.Tests.Mediator
{
    public class ServiceProviderTests
    {
        [Fact]
        public void GetService()
        {
            var testServiceProvider = new TestServiceProvider();
            var serviceProvider = new Handyman.Mediator.ServiceProvider(testServiceProvider);
            serviceProvider.GetService(typeof(bool));
            testServiceProvider.Type.ShouldBe(typeof(bool));

            new Handyman.Mediator.ServiceProvider(t => t).GetService(typeof(int)).ShouldBe(typeof(int));

            new Handyman.Mediator.ServiceProvider(t => t, null).GetService(typeof(int)).ShouldBe(typeof(int));
        }

        [Fact]
        public void GetServices()
        {
            var testServiceProvider = new TestServiceProvider();
            var serviceProvider = new Handyman.Mediator.ServiceProvider(testServiceProvider);
            serviceProvider.GetServices(typeof(bool));
            testServiceProvider.Type.ShouldBe(typeof(IEnumerable<bool>));

            new Handyman.Mediator.ServiceProvider(t => new[] { Activator.CreateInstance(t.GenericTypeArguments.Single()) })
                .GetServices(typeof(int))
                .ShouldBe(new object[] { 0 });

            new Handyman.Mediator.ServiceProvider(null, t => new[] { Activator.CreateInstance(t) })
                .GetServices(typeof(int))
                .ShouldBe(new object[] { 0 });
        }

        class TestServiceProvider : System.IServiceProvider
        {
            public Type Type { get; private set; }

            public object GetService(Type serviceType)
            {
                Type = serviceType;
                return serviceType.IsGenericType
                    ? new[] { Activator.CreateInstance(serviceType.GetGenericArguments().Single()) }
                    : Activator.CreateInstance(serviceType);
            }
        }
    }
}