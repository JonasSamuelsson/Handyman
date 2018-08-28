using System;

namespace Handyman.Mediator
{
    public class ServiceProvider : IServiceProvider
    {
        private readonly Func<Type, object> _func;

        public ServiceProvider(Func<Type,object> func)
        {
            _func = func;
        }

        public object GetService(Type serviceType)
        {
            return _func(serviceType);
        }
    }
}