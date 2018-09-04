using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Handyman.Mediator.Internals
{
    internal class PipelinedEventHandlerFactoryBuilder : IEventHandlerFactoryBuilder
    {
        private static readonly ConcurrentDictionary<Type, Func<Func<Type, object>, object>> Cache = new ConcurrentDictionary<Type, Func<Func<Type, object>, object>>();

        public Func<Func<Type, object>, object> BuildFactory(Type eventType)
        {
            return Cache.GetOrAdd(eventType, _ => CompileFactory(eventType));
        }

        private Func<Func<Type, object>, object> CompileFactory(Type eventType)
        {
            var serviceProvider = Expression.Parameter(typeof(Func<Type, object>));

            var pipelineHandlerType = typeof(IEventPipelineHandler<>).MakeGenericType(eventType);
            var pipelineHandlersType = typeof(IEnumerable<>).MakeGenericType(pipelineHandlerType);
            var pipelineHandlers = Expression.Invoke(serviceProvider, Expression.Constant(pipelineHandlersType));
            var typedPipelineHandlers = Expression.Convert(pipelineHandlers, pipelineHandlersType);

            var innerHandlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            var innerHandlersType = typeof(IEnumerable<>).MakeGenericType(innerHandlerType);
            var innerHandlers = Expression.Invoke(serviceProvider, Expression.Constant(innerHandlersType));
            var typedInnerHandlers = Expression.Convert(innerHandlers, innerHandlersType);

            var handlerType = typeof(PipelinedEventHandler<>).MakeGenericType(eventType);
            var handlerCtor = handlerType.GetConstructors().Single();
            var handler = Expression.New(handlerCtor, typedPipelineHandlers, typedInnerHandlers);

            return Expression.Lambda<Func<Func<Type, object>, object>>(handler, serviceProvider).Compile();
        }
    }
}