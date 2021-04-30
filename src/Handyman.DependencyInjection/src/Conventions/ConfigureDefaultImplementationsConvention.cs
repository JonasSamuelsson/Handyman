using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DependencyInjection.Conventions
{
    public class ConfigureDefaultImplementationsConvention : IServiceConfigurationConvention
    {
        private static readonly ServiceConfigurationPolicy DefaultPolicy = ServiceConfigurationPolicy.TryAdd;
        private static readonly ServiceLifetime DefaultLifetime = ServiceLifetime.Transient;

        private readonly ServiceConfigurationOptions _options;

        public ConfigureDefaultImplementationsConvention(ServiceConfigurationOptions options)
        {
            _options = options;
        }

        public void Execute(IReadOnlyCollection<Type> types, IServiceCollection services)
        {
            var interfaces = types.Where(x => x.IsInterface);
            var classes = types.Where(x => x.IsConcreteClass()).ToList();

            var policy = _options.ConfigurationPolicy ?? DefaultPolicy;

            foreach (var @interface in interfaces)
            {
                var @class = classes.FirstOrDefault(c => @interface.Namespace == c.Namespace && @interface.Name.Substring(1) == c.Name);

                if (@class == null)
                    continue;

                if (!@interface.IsAssignableFrom(@class))
                    continue;

                var lifetime = ServiceLifetimeProvider.GetLifetimeOrNullFromAttribute(@class) ?? _options.Lifetime ?? DefaultLifetime;

                var serviceDescriptor = new ServiceDescriptor(@interface, @class, lifetime);

                ServiceConfigurationUtility
                    .Configure(services, serviceDescriptor, policy);
            }
        }
    }
}