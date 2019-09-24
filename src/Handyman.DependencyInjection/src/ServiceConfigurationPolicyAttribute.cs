using System;

namespace Handyman.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ServiceConfigurationPolicyAttribute : Attribute
    {
        private readonly Type _serviceRegistrationPolicyType;

        public ServiceConfigurationPolicyAttribute(Type serviceRegistrationPolicyType)
        {
            _serviceRegistrationPolicyType = serviceRegistrationPolicyType;
        }

        public IServiceConfigurationPolicy GetServiceRegistrationPolicy()
        {
            return (IServiceConfigurationPolicy)Activator.CreateInstance(_serviceRegistrationPolicyType);
        }
    }
}