using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Handyman.Mediator.Internals
{
    internal class EventHandlerFactoryBuilder : IEventHandlerFactoryBuilder
    {
        private static readonly ConcurrentDictionary<Type, Func<ServiceProvider, object>> Cache = new ConcurrentDictionary<Type, Func<ServiceProvider, object>>();

        public Func<ServiceProvider, object> BuildFactory(Type eventType)
        {
            return Cache.GetOrAdd(eventType, _ => CompileFactory(eventType));
        }

        private Func<ServiceProvider, object> CompileFactory(Type eventType)
        {
            var serviceProvider = Expression.Parameter(typeof(ServiceProvider));

            var innerHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            var innerHandlersType = typeof(IEnumerable<>).MakeGenericType(innerHandlerType);
            var innerHandlers = Expression.Invoke(serviceProvider, Expression.Constant(innerHandlersType));
            var typedInnerHandlers = Expression.Convert(innerHandlers, innerHandlersType);

            var handlerType = typeof(EventHandler<>).MakeGenericType(eventType);
            var handlerCtor = handlerType.GetConstructors().Single();
            var handler = Expression.New(handlerCtor, typedInnerHandlers);

            return Expression.Lambda<Func<ServiceProvider, object>>(handler, serviceProvider).Compile();
        }
    }
}