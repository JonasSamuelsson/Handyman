using System;
using System.Linq;
using System.Linq.Expressions;

namespace Handyman.Mediator
{
    internal static class RequestHandlerFactoryBuilder
    {
        internal static Func<IServiceProvider, object> Create<TResponse>(Type requestType)
        {
            var responseType = typeof(TResponse);
            var pipelineType = GetPipelineType(requestType, responseType);
            var pipelineHandlerType = GetPipelineHandlerType(requestType, responseType);
            var handlerType = GetHandlerType(requestType, responseType);
            return CreatePipelineFactory(pipelineType, pipelineHandlerType, handlerType);
        }

        private static Type GetPipelineType(Type requestType, Type responseType)
        {
            return typeof(DelegatingRequestHandler<,>).MakeGenericType(requestType, responseType);
        }

        private static Type GetPipelineHandlerType(Type requestType, Type responseType)
        {
            return typeof(IRequestPipelineHandler<,>).MakeGenericType(requestType, responseType);
        }

        private static Type GetHandlerType(Type requestType, Type responseType)
        {
            return typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        }

        private static Func<IServiceProvider, object> CreatePipelineFactory(Type pipelineType, Type pipelineHandlerType, Type handlerType)
        {
            var serviceProvider = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

            var getServicesMethod = typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetServices));
            var pipelineHandlers = Expression.Call(serviceProvider, getServicesMethod, Expression.Constant(pipelineHandlerType));
            var castMethod = typeof(Enumerable).GetMethod(nameof(Enumerable.Cast)).MakeGenericMethod(pipelineHandlerType);
            var typedPipelineHandlers = Expression.Call(castMethod, pipelineHandlers);

            var getServiceMethod = typeof(System.IServiceProvider).GetMethod(nameof(IServiceProvider.GetService));
            var handler = Expression.Call(serviceProvider, getServiceMethod, Expression.Constant(handlerType));
            var typedHandler = Expression.Convert(handler, handlerType);

            var ctor = pipelineType.GetConstructors().Single();
            var pipeline = Expression.New(ctor, typedPipelineHandlers, typedHandler);

            return Expression.Lambda<Func<IServiceProvider, object>>(pipeline, serviceProvider).Compile();
        }
    }
}