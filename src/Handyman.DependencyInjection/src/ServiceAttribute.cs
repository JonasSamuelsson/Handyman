using Microsoft.Extensions.DependencyInjection;
using System;

namespace Handyman.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ServiceAttribute : Attribute
    {
        public ServiceAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }

        public ServiceAttribute(Type serviceType, ServiceLifetime serviceLifetime)
        {
            ServiceType = serviceType;
            ServiceLifetime = serviceLifetime;
        }

        public ServiceAttribute(Type serviceType, ServiceConfigurationPolicy serviceConfigurationPolicy)
        {
            ServiceType = serviceType;
            ServiceConfigurationPolicy = serviceConfigurationPolicy;
        }

        public ServiceAttribute(Type serviceType, ServiceLifetime serviceLifetime, ServiceConfigurationPolicy serviceConfigurationPolicy)
        {
            ServiceType = serviceType;
            ServiceLifetime = serviceLifetime;
            ServiceConfigurationPolicy = serviceConfigurationPolicy;
        }

        public Type ServiceType { get; }
        public ServiceLifetime? ServiceLifetime { get; }
        public ServiceConfigurationPolicy? ServiceConfigurationPolicy { get; }
    }
}