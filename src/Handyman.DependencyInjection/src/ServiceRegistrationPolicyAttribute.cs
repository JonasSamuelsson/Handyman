using System;

namespace Handyman.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ServiceRegistrationPolicyAttribute : Attribute
    {
        private readonly Type _serviceRegistrationPolicyType;

        public ServiceRegistrationPolicyAttribute(Type serviceRegistrationPolicyType)
        {
            _serviceRegistrationPolicyType = serviceRegistrationPolicyType;
        }

        public IServiceRegistrationPolicy GetServiceRegistrationPolicy()
        {
            return (IServiceRegistrationPolicy)Activator.CreateInstance(_serviceRegistrationPolicyType);
        }
    }
}