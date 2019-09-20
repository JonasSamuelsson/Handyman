using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Handyman.DependencyInjection
{
    public class ServiceLifetimeProvider
    {
        public static ServiceLifetime GetLifetimeOrDefault(Type type, ServiceLifetime defaultLifetime)
        {
            return type.GetCustomAttribute<ServiceLifetimeAttribute>()?.Lifetime ?? defaultLifetime;
        }
    }
}