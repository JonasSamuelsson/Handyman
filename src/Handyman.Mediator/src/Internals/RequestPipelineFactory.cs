using Handyman.Mediator.RequestPipelineCustomization;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Handyman.Mediator.Internals
{
    internal class RequestPipelineFactory
    {
        private readonly ConcurrentDictionary<Type, Func<object>> _factoryMethods = new ConcurrentDictionary<Type, Func<object>>();

        internal RequestPipeline<TResponse> GetPipeline<TResponse>(IRequest<TResponse> request, IServiceProvider serviceProvider)
        {
            var requestType = request.GetType();
            var factoryMethod = _factoryMethods.GetOrAdd(requestType, type => CreateFactory<TResponse>(type, serviceProvider));
            return (RequestPipeline<TResponse>)factoryMethod.Invoke();
        }

        private static Func<object> CreateFactory<TResponse>(Type requestType, IServiceProvider serviceProvider)
        {
            var responseType = typeof(TResponse);
            var factoryMethodBuilderType = typeof(FactoryMethodBuilder<,>).MakeGenericType(requestType, responseType);
            var factoryMethodBuilder = (FactoryMethodBuilder)Activator.CreateInstance(factoryMethodBuilderType);
            return factoryMethodBuilder.CreateFactoryMethod(serviceProvider);
        }

        private abstract class FactoryMethodBuilder
        {
            internal abstract Func<object> CreateFactoryMethod(IServiceProvider serviceProvider);
        }

        private class FactoryMethodBuilder<TRequest, TResponse> : FactoryMethodBuilder
            where TRequest : IRequest<TResponse>
        {
            internal override Func<object> CreateFactoryMethod(IServiceProvider serviceProvider)
            {
                var attributes = typeof(TRequest).GetCustomAttributes<RequestPipelineBuilderAttribute>().ToListOptimized();

                if (attributes.Count == 0)
                {
                    return () => DefaultRequestPipeline<TRequest, TResponse>.Instance;
                }

                return CreateCustomizedPipeline;

                RequestPipeline<TResponse> CreateCustomizedPipeline()
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