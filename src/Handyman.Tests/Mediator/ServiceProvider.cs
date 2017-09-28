using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tests.Mediator
{
    internal class ServiceProvider
    {
        private readonly Dictionary<Type, List<Type>> _dictionary = new Dictionary<Type, List<Type>>();

        public object GetService(Type handlerType)
        {
            return GetServices(handlerType).Single();
        }

        public IEnumerable<object> GetServices(Type handlerType)
        {
            return _dictionary[handlerType].Select(Activator.CreateInstance);
        }

        public void Add<TService, TImplementation>() where TImplementation : TService
        {
            if (!_dictionary.TryGetValue(typeof(TService), out var list))
                _dictionary[typeof(TService)] = list = new List<Type>();
            list.Add(typeof(TImplementation));
        }
    }
}