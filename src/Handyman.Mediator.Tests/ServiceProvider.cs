using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Mediator.Tests
{
    internal class ServiceProvider : IServiceProvider
    {
        private readonly Dictionary<Type, List<Func<object>>> _dictionary = new Dictionary<Type, List<Func<object>>>();

        public object GetService(Type handlerType)
        {
            return GetServices(handlerType).Single();
        }

        public IEnumerable<object> GetServices(Type handlerType)
        {
            return _dictionary[handlerType].Select(factory => factory.Invoke());
        }

        public void Add<TService, TImplementation>() where TImplementation : TService
        {
            Add(typeof(TService), () => Activator.CreateInstance(typeof(TImplementation)));
        }

        public void Add<TService>(Func<TService> factory)
        {
            Add(typeof(TService), () => factory());
        }

        public void Add(Type type, Func<object> factory)
        {
            if (!_dictionary.TryGetValue(type, out var list))
            {
                list = new List<Func<object>>();
                _dictionary[type] = list;
            }

            list.Add(factory);
        }
    }
}