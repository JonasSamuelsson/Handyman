using System;
using System.Linq;
using System.Linq.Expressions;

namespace Handyman.Mediator
{
    internal static class RequestHandlerFactoryBuilder
    {
        internal static Func<IServiceProvider, object> Create<TResponse>(Type requestType, bool useRequestPipeline)
        {
            var responseType = typeof(TResponse);
            return useRequestPipeline
                ? CreatePipelineHandlerFactory(requestType, responseType)
                : CreateHandlerFactory(requestType, responseType);
        }

        private static Func<IServiceProvider, object> CreatePipelineHandlerFactory(Type requestType, Type responseType)
        {
            var handlerType = typeof(PipelinedRequestHandler<,>).MakeGenericType(requestType, responseType);
            var pipelineHandlerType = typeof(IRequestPipelineHandler<,>).MakeGenericType(requestType, responseType);
            var innerHandlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

            var serviceProvider = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

            var getServicesMethod = typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetServices));
            var pipelineHandlers = Expression.Call(serviceProvider, getServicesMethod, Expression.Constant(pipelineHandlerType));
            var castMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast)).MakeGenericMethod(pipelineHandlerType);
            var typedPipelineHandlers = Expression.Call(castMethod, pipelineHandlers);

            var getServiceMethod = typeof(System.IServiceProvider).GetMethod(nameof(IServiceProvider.GetService));
            var innerHandler = Expression.Call(serviceProvider, getServiceMethod, Expression.Constant(innerHandlerType));
            var typedInnerHandler = Expression.Convert(innerHandler, innerHandlerType);

            var handlerCtor = handlerType.GetConstructors().Single();
            var handler = Expression.New(handlerCtor, typedPipelineHandlers, typedInnerHandler);

            return Expression.Lambda<Func<IServiceProvider, object>>(handler, serviceProvider).Compile();
        }

        private static Func<IServiceProvider, object> CreateHandlerFactory(Type requestType, Type responseType)
        {
            var handlerType = typeof(RequestHandler<,>).MakeGenericType(requestType, responseType);
            var innerHandlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

            var serviceProvider = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

            var getServiceMethod = typeof(System.IServiceProvider).GetMethod(nameof(IServiceProvider.GetService));
            var innerHandler = Expression.Call(serviceProvider, getServiceMethod, Expression.Constant(innerHandlerType));
            var typedInnerHandler = Expression.Convert(innerHandler, innerHandlerType);

            var handlerCtor = handlerType.GetConstructors().Single();
            var handler = Expression.New(handlerCtor, typedInnerHandler);

            return Expression.Lambda<Func<IServiceProvider, object>>(handler, serviceProvider).Compile();
        }
    }
}