using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Handyman.DependencyInjection.Conventions
{
    public static class ServiceConfigurationUtility
    {
        public static void Configure(IServiceCollection services, Type serviceType, Type implementationType,
            ServiceLifetime lifetime, ServiceConfigurationPolicy policy)
        {
            var descriptor = new ServiceDescriptor(serviceType, implementationType, lifetime);

            Configure(services, descriptor, policy);
        }

        public static void Configure(IServiceCollection services, ServiceDescriptor descriptor,
            ServiceConfigurationPolicy policy)
        {
            switch (policy)
            {
                case ServiceConfigurationPolicy.Add:
                    services.Add(descriptor);
                    break;
                case ServiceConfigurationPolicy.TryAdd:
                    services.TryAdd(descriptor);
                    break;
                case ServiceConfigurationPolicy.TryAddEnumerable:
                    services.TryAddEnumerable(descriptor);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(policy), policy, null);
            }
        }
    }
}