using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handyman.DependencyInjection.Conventions
{
    public class ConfigureClassesWithServiceAttributeConvention : IServiceConfigurationConvention
    {
        public void Execute(IReadOnlyCollection<Type> types, IServiceCollection services)
        {
            foreach (var type in types.Where(x => x.IsClass))
            {
                var attributes = type.GetCustomAttributes<ServiceAttribute>(false);

                foreach (var attribute in attributes)
                {
                    services.Add(new ServiceDescriptor(attribute.ServiceType, type, attribute.ServiceLifetime));
                }
            }
        }
    }
}