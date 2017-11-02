using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Mediator
{
    public class ServiceProvider : IServiceProvider
    {
        private readonly Func<Type, object> _getService;
        private readonly Func<Type, IEnumerable<object>> _getServices;

        public ServiceProvider(System.IServiceProvider serviceProvider):this(serviceProvider.GetService)
        {
        }

        public ServiceProvider(Func<Type, object> getService)
        {
            _getService = getService;
            _getServices = t =>
            {
                var serviceType = typeof(IEnumerable<>).MakeGenericType(t);
                var services = (IEnumerable)GetService(serviceType);
                return services.Cast<object>();
            };
        }

        public ServiceProvider(Func<Type, object> getService, Func<Type, IEnumerable<object>> getServices)
        {
            _getService = getService;
            _getServices = getServices;
        }

        public object GetService(Type serviceType)
        {
            return _getService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _getServices(serviceType);
        }
    }
}