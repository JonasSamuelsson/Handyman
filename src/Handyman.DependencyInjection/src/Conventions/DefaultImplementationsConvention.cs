using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Handyman.DependencyInjection.Conventions
{
    public class DefaultImplementationsConvention : IConvention
    {
        private readonly ServiceLifetime _serviceLifetime;

        public DefaultImplementationsConvention(ServiceLifetime? serviceLifetime)
        {
            _serviceLifetime = serviceLifetime ?? ServiceLifetime.Transient;
        }

        public void Execute(IReadOnlyCollection<Type> types, IServiceCollection services)
        {
            var interfaces = types.Where(x => x.IsInterface);
            var classes = types.Where(x => x.IsConcreteClass()).ToList();

            foreach (var @interface in interfaces)
            {
                var @class = classes.FirstOrDefault(c => @interface.Namespace == c.Namespace && @interface.Name.Substring(1) == c.Name);

                if (@class == null) continue;

                var lifetime = ServiceLifetimeProvider.GetLifetimeOrDefault(@class, _serviceLifetime);

                services.Add(new ServiceDescriptor(@interface, @class, lifetime));
            }
        }
    }
}