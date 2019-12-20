using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Handyman.DependencyInjection
{
    public class ServiceLifetimeProvider
    {
        public static ServiceLifetime? GetLifetimeOrNullFromAttribute(Type type)
        {
            return type.GetCustomAttribute<ServiceLifetimeAttribute>()?.Lifetime;
        }
    }
}