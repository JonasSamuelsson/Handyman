using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Handyman.DependencyInjection.Tests
{
    public class RegistrationPolicyTests
    {
        [Fact]
        public void ShouldRegisterServicesUsingRegistrationPolicy()
        {
            var services = new ServiceCollection();

            services.Scan(_ =>
            {
                _.Types(GetType().GetNestedTypes(BindingFlags.NonPublic));
                _.UsingRegistrationPolicies();
            });

            services.BuildServiceProvider().GetRequiredService<IFoo>().ShouldBeOfType<Foo>();
        }

        private interface IFoo { }

        [ServiceRegistrationPolicy(typeof(ServiceRegistrationPolicy))]
        private class Foo : IFoo { }

        private class ServiceRegistrationPolicy : IServiceRegistrationPolicy
        {
            public void Register(Type type, IServiceCollection services)
            {
                services.AddTransient(typeof(IFoo), type);
            }
        }
    }
}