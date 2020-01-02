using Handyman.Mediator.RequestPipelineCustomization;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Handyman.Mediator.Internals
{
    internal class RequestPipelineFactory
    {
        private readonly ConcurrentDictionary<Type, Func<IServiceProvider, object>> _factoryMethods = new ConcurrentDictionary<Type, Func<IServiceProvider, object>>();

        internal RequestPipeline<TResponse> GetPipeline<TResponse>(IRequest<TResponse> request, IServiceProvider serviceProvider)
        {
            var requestType = request.GetType();
            var responseType = typeof(TResponse);
            var factoryMethod = _factoryMethods.GetOrAdd(requestType, _ => CreateFactoryMethod(requestType, responseType));
            return (RequestPipeline<TResponse>)factoryMethod.Invoke(serviceProvider);
        }

        private static Func<IServiceProvider, object> CreateFactoryMethod(Type requestType, Type responseType)
        {
            var factoryMethodBuilderType = typeof(FactoryMethodBuilder<,>).MakeGenericType(requestType, responseType);
            var factoryMethodBuilder = (FactoryMethodBuilder)Activator.CreateInstance(factoryMethodBuilderType);
            return factoryMethodBuilder.CreateFactoryMethod();
        }

        private abstract class FactoryMethodBuilder
        {
            internal abstract Func<IServiceProvider, object> CreateFactoryMethod();
        }

        private class FactoryMethodBuilder<TRequest, TResponse> : FactoryMethodBuilder
            where TRequest : IRequest<TResponse>
        {
            internal override Func<IServiceProvider, object> CreateFactoryMethod()
            {
                var attributes = typeof(TRequest).GetCustomAttributes<RequestPipelineBuilderAttribute>().ToListOptimized();

                if (attributes.Count == 0)
                {
                    return _ => DefaultRequestPipeline<TRequest, TResponse>.Instance;
                }

                return CreateCustomizedPipeline;

                object CreateCustomizedPipeline(IServiceProvider serviceProvider)
                {
                    var builder = new RequestPipelineBuilder();

                    foreach (var attribute in attributes)
                    {
                        attribute.Configure(builder, serviceProvider);
                    }

                    return new CustomizedRequestPipeline<TRequest, TResponse>
                    {
                        FilterSelectors = builder.FilterSelectors,
                        HandlerSelectors = builder.HandlerSelectors,
                        HandlerExecutionStrategy = builder.HandlerExecutionStrategy ?? DefaultRequestHandlerExecutionStrategy.Instance
                    };
                }
            }
        }
    }
}