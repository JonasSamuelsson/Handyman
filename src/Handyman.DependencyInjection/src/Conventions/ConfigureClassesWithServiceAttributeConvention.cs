using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handyman.DependencyInjection.Conventions
{
    public class ConfigureClassesWithServiceAttributeConvention : IServiceConfigurationConvention
    {
        private static readonly ServiceLifetime DefaultLifetime = ServiceLifetime.Transient;

        private readonly ServiceConfigurationOptions _options;

        public ConfigureClassesWithServiceAttributeConvention(ServiceConfigurationOptions options)
        {
            _options = options;
        }

        public void Execute(IReadOnlyCollection<Type> types, IServiceCollection services)
        {
            foreach (var type in types.Where(x => x.IsClass))
            {
                var attributes = type.GetCustomAttributes<ServiceAttribute>(false);

                foreach (var attribute in attributes)
                {
                    var serviceType = attribute.ServiceType;
                    var lifetime = attribute.ServiceLifetime ?? _options.ServiceLifetime ?? DefaultLifetime;
                    var policy = GetPolicy(attribute, serviceType, type);

                    var serviceDescriptor = new ServiceDescriptor(serviceType, type, lifetime);

                    ServiceConfigurationUtility
                        .Configure(services, serviceDescriptor, policy);
                }
            }
        }

        private ServiceConfigurationPolicy GetPolicy(ServiceAttribute attribute, Type serviceType, Type implementationType)
        {
            if (attribute.ServiceConfigurationPolicy.HasValue)
            {
                return attribute.ServiceConfigurationPolicy.Value;
            }

            if (_options.ServiceConfigurationPolicy.HasValue)
            {
                return _options.ServiceConfigurationPolicy.Value;
            }

            if (serviceType == implementationType)
            {
                return ServiceConfigurationPolicy.TryAdd;
            }

            return ServiceConfigurationPolicy.TryAddEnumerable;
        }
    }
}