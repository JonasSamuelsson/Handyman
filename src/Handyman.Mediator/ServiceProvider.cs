using System;
using System.Collections.Generic;

namespace Handyman.Mediator
{
    internal class ServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        internal ServiceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        internal object GetService(Type type)
        {
            return _serviceProvider.GetService(type)
                   ?? throw new InvalidOperationException($"Could not get service of type '{type.FullName}'.");
        }

        internal IEnumerable<object> GetServices(Type type)
        {
            return (IEnumerable<object>)GetService(typeof(IEnumerable<>).MakeGenericType(type));
        }
    }
}