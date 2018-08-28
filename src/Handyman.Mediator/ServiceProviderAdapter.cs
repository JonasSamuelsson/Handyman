using System;
using System.Collections.Generic;

namespace Handyman.Mediator
{
    internal class ServiceProviderAdapter
    {
        private readonly IServiceProvider _serviceProvider;

        internal ServiceProviderAdapter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        internal object GetService(Type type)
        {
            return _serviceProvider.GetService(type) ?? throw CreateException(type);
        }

        private static InvalidOperationException CreateException(Type type)
        {
            return new InvalidOperationException($"Could not get service of type '{type.FullName}'.");
        }

        internal IEnumerable<object> GetServices(Type type)
        {
            return (IEnumerable<object>)GetService(typeof(IEnumerable<>).MakeGenericType(type));
        }
    }
}