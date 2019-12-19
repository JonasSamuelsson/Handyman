using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DependencyInjection.Conventions
{
    internal class ExecuteServiceConfigurationStrategiesConvention : IConvention
    {
        public void Execute(IReadOnlyCollection<Type> types, IServiceCollection services)
        {
            foreach (var type in types)
            {
                if (type.IsGenericType)
                    continue;

                if (!type.IsConcreteClass())
                    continue;

                if (type.GetInterfaces().All(x => x != typeof(IServiceConfigurationStrategy)))
                    continue;

                ((IServiceConfigurationStrategy)Activator.CreateInstance(type)).Execute(services);
            }
        }
    }
}