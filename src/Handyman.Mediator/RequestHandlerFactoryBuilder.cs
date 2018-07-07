using System;
using System.Linq;
using System.Linq.Expressions;
using BindingFlags = System.Reflection.BindingFlags;

namespace Handyman.Mediator
{
    internal static class RequestHandlerFactoryBuilder
    {
        private const BindingFlags Flags = BindingFlags.Instance | BindingFlags.NonPublic;

        internal static Func<ServiceProvider, object> Create<TResponse>(Type requestType, bool useRequestPipeline)
        {
            var responseType = typeof(TResponse);
            return useRequestPipeline
                ? CreatePipelineHandlerFactory(requestType, responseType)
                : CreateHandlerFactory(requestType, responseType);
        }

        private static Func<ServiceProvider, object> CreatePipelineHandlerFactory(Type requestType, Type responseType)
        {
            var handlerType = typeof(PipelinedRequestHandler<,>).MakeGenericType(requestType, responseType);
            var pipelineHandlerType = typeof(IRequestPipelineHandler<,>).MakeGenericType(requestType, responseType);
            var innerHandlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

            var serviceProvider = Expression.Parameter(typeof(ServiceProvider), "serviceProvider");

            var getServicesMethod = typeof(ServiceProvider).GetMethod(nameof(ServiceProvider.GetServices), Flags);
            var pipelineHandlers = Expression.Call(serviceProvider, getServicesMethod, Expression.Constant(pipelineHandlerType));
            var castMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast)).MakeGenericMethod(pipelineHandlerType);
            var typedPipelineHandlers = Expression.Call(castMethod, pipelineHandlers);

            var getServiceMethod = typeof(ServiceProvider).GetMethod(nameof(ServiceProvider.GetService), Flags);
            var innerHandler = Expression.Call(serviceProvider, getServiceMethod, Expression.Constant(innerHandlerType));
            var typedInnerHandler = Expression.Convert(innerHandler, innerHandlerType);

            var handlerCtor = handlerType.GetConstructors().Single();
            var handler = Expression.New(handlerCtor, typedPipelineHandlers, typedInnerHandler);

            return Expression.Lambda<Func<ServiceProvider, object>>(handler, serviceProvider).Compile();
        }

        private static Func<ServiceProvider, object> CreateHandlerFactory(Type requestType, Type responseType)
        {
            var handlerType = typeof(RequestHandler<,>).MakeGenericType(requestType, responseType);
            var innerHandlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

            var serviceProvider = Expression.Parameter(typeof(ServiceProvider), "serviceProvider");

            var getServiceMethod = typeof(ServiceProvider).GetMethod(nameof(ServiceProvider.GetService), Flags);
            var innerHandler = Expression.Call(serviceProvider, getServiceMethod, Expression.Constant(innerHandlerType));
            var typedInnerHandler = Expression.Convert(innerHandler, innerHandlerType);

            var handlerCtor = handlerType.GetConstructors().Single();
            var handler = Expression.New(handlerCtor, typedInnerHandler);

            return Expression.Lambda<Func<ServiceProvider, object>>(handler, serviceProvider).Compile();
        }
    }
}