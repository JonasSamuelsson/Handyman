using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Reflection;
using Xunit;

namespace Handyman.DependencyInjection.Tests
{
    public class ServiceConfigurationPolicyTests
    {
        [Fact]
        public void ShouldConfigureServicesUsingPolicy()
        {
            var services = new ServiceCollection();

            services.Scan(_ =>
            {
                _.Types(GetType().GetNestedTypes(BindingFlags.NonPublic));
                _.UsingServiceConfigurationPolicies();
            });

            services.BuildServiceProvider().GetRequiredService<IFoo>().ShouldBeOfType<Foo>();
        }

        private interface IFoo { }

        [ServiceConfigurationPolicy(typeof(ServiceConfigurationPolicy))]
        private class Foo : IFoo { }

        private class ServiceConfigurationPolicy : IServiceConfigurationPolicy
        {
            public void Register(Type type, IServiceCollection services)
            {
                services.AddTransient(typeof(IFoo), type);
            }
        }
    }
}