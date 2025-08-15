using System;
using System.Collections.Generic;

namespace Handyman.Mediator
{
    internal static class ServiceProviderExtensions
    {
        public static T GetRequiredService<T>(this IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetService<T>();

            if (service != null)
                return service;

            var message = $"No service for type '{typeof(T).FullName}' has been registered.";
            throw new InvalidOperationException(message);
        }

        public static T? GetService<T>(this IServiceProvider serviceProvider)
        {
            return (T?)serviceProvider.GetService(typeof(T));
        }

        public static IEnumerable<object> GetServices(this IServiceProvider serviceProvider, Type type)
        {
            return (IEnumerable<object>)serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(type))!;
        }

        public static IEnumerable<T> GetServices<T>(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IEnumerable<T>>();
        }
    }
}