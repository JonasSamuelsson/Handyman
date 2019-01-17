using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;

namespace Handyman.Mediator.Internals
{
    internal class RequestHandlerFactoryBuilder : IRequestHandlerFactoryBuilder
    {
        private static readonly ConcurrentDictionary<Type, Func<ServiceProvider, object>> Cache = new ConcurrentDictionary<Type, Func<ServiceProvider, object>>();

        public Func<ServiceProvider, object> BuildFactory(Type requestType, Type responseType)
        {
            return Cache.GetOrAdd(requestType, _ => CompileFactory(requestType, responseType));
        }

        private static Func<ServiceProvider, object> CompileFactory(Type requestType, Type responseType)
        {
            var serviceProvider = Expression.Parameter(typeof(ServiceProvider));

            var innerHandlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
            var innerHandler = Expression.Invoke(serviceProvider, Expression.Constant(innerHandlerType));
            var typedInnerHandler = Expression.Convert(innerHandler, innerHandlerType);

            var handlerType = typeof(RequestHandler<,>).MakeGenericType(requestType, responseType);
            var handlerCtor = handlerType.GetConstructors().Single();
            var handler = Expression.New(handlerCtor, typedInnerHandler);

            return Expression.Lambda<Func<ServiceProvider, object>>(handler, serviceProvider).Compile();
        }
    }
}