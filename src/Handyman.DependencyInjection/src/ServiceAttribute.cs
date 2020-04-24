using Microsoft.Extensions.DependencyInjection;
using System;

namespace Handyman.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute(Type serviceType)
            : this(serviceType, ServiceLifetime.Transient)
        {
        }

        public ServiceAttribute(Type serviceType, ServiceLifetime serviceLifetime)
        {
            ServiceType = serviceType;
            ServiceLifetime = serviceLifetime;
        }

        public Type ServiceType { get; }
        public ServiceLifetime ServiceLifetime { get; }
    }
}