using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tests.Mediator
{
    internal class ServiceProvider
    {
        private readonly Dictionary<Type, IEnumerable<Type>> _dictionary;

        public ServiceProvider(Type serviceType, params Type[] instanceTypes)
        {
            _dictionary = new Dictionary<Type, IEnumerable<Type>>
            {
                [serviceType] = instanceTypes
            };
        }

        public ServiceProvider(Dictionary<Type, IEnumerable<Type>> dictionary)
        {
            _dictionary = dictionary;
        }

        public object GetService(Type handlerType)
        {
            return GetServices(handlerType).Single();
        }

        public IEnumerable<object> GetServices(Type handlerType)
        {
            return _dictionary[handlerType].Select(Activator.CreateInstance);
        }
    }
}