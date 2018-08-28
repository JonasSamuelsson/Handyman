using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using BindingFlags = System.Reflection.BindingFlags;

namespace Handyman.Mediator
{
    internal static class RequestHandlerFactoryBuilder
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

        private static readonly ConcurrentDictionary<bool, ConcurrentDictionary<Type, Func<ServiceProviderAdapter, object>>> Cache = new ConcurrentDictionary<bool, ConcurrentDictionary<Type, Func<ServiceProviderAdapter, object>>>();

        internal static Func<ServiceProviderAdapter, object> Create<TResponse>(Type requestType, bool useRequestPipeline)
        {
            var cache = Cache.GetOrAdd(useRequestPipeline, _ => new ConcurrentDictionary<Type, Func<ServiceProviderAdapter, object>>());
            return cache.GetOrAdd(requestType, _ => CreateFactory(useRequestPipeline, requestType, typeof(TResponse)));
        }

        private static Func<ServiceProviderAdapter, object> CreateFactory(bool useRequestPipeline, Type requestType, Type responseType)
        {
            return useRequestPipeline
                ? CreatePipelineHandlerFactory(requestType, responseType)
                : CreateHandlerFactory(requestType, responseType);
        }

        private static Func<ServiceProviderAdapter, object> CreatePipelineHandlerFactory(Type requestType, Type responseType)
        {
            var handlerType = typeof(PipelinedRequestHandler<,>).MakeGenericType(requestType, responseType);
            var pipelineHandlerType = typeof(IRequestPipelineHandler<,>).MakeGenericType(requestType, responseType);
            var innerHandlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

            var serviceProvider = Expression.Parameter(typeof(ServiceProviderAdapter), "serviceProvider");

            var getServicesMethod = typeof(ServiceProviderAdapter).GetMethod(nameof(ServiceProviderAdapter.GetServices), Flags);
            var pipelineHandlers = Expression.Call(serviceProvider, getServicesMethod, Expression.Constant(pipelineHandlerType));
            var castMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast)).MakeGenericMethod(pipelineHandlerType);
            var typedPipelineHandlers = Expression.Call(castMethod, pipelineHandlers);

            var getServiceMethod = typeof(ServiceProviderAdapter).GetMethod(nameof(ServiceProviderAdapter.GetService), Flags);
            var innerHandler = Expression.Call(serviceProvider, getServiceMethod, Expression.Constant(innerHandlerType));
            var typedInnerHandler = Expression.Convert(innerHandler, innerHandlerType);

            var handlerCtor = handlerType.GetConstructors().Single();
            var handler = Expression.New(handlerCtor, typedPipelineHandlers, typedInnerHandler);

            return Expression.Lambda<Func<ServiceProviderAdapter, object>>(handler, serviceProvider).Compile();
        }

        private static Func<ServiceProviderAdapter, object> CreateHandlerFactory(Type requestType, Type responseType)
        {
            var handlerType = typeof(RequestHandler<,>).MakeGenericType(requestType, responseType);
            var innerHandlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

            var serviceProvider = Expression.Parameter(typeof(ServiceProviderAdapter), "serviceProvider");

            var getServiceMethod = typeof(ServiceProviderAdapter).GetMethod(nameof(ServiceProviderAdapter.GetService), Flags);
            var innerHandler = Expression.Call(serviceProvider, getServiceMethod, Expression.Constant(innerHandlerType));
            var typedInnerHandler = Expression.Convert(innerHandler, innerHandlerType);

            var handlerCtor = handlerType.GetConstructors().Single();
            var handler = Expression.New(handlerCtor, typedInnerHandler);

            return Expression.Lambda<Func<ServiceProviderAdapter, object>>(handler, serviceProvider).Compile();
        }
    }
}