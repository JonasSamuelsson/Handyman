using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Handyman.DependencyInjection.Conventions
{
    public class RegistrationPolicyConvention : IConvention
    {
        public void Execute(IReadOnlyCollection<Type> types, IServiceCollection services)
        {
            foreach (var type in types)
            {
                foreach (var attribute in type.GetCustomAttributes<ServiceRegistrationPolicyAttribute>())
                {
                    attribute.GetServiceRegistrationPolicy().Register(type, services);
                }
            }
        }
    }
}