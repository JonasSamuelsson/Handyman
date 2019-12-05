using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Handyman.Mediator.RequestPipelineCustomization;

namespace Handyman.Mediator.Internals
{
    internal static class RequestPipelineFactory
    {
        private static readonly ConcurrentDictionary<Type, Func<object>> Cache = new ConcurrentDictionary<Type, Func<object>>();

        internal static RequestPipeline<TResponse> GetRequestPipeline<TResponse>(IRequest<TResponse> request,
            IServiceProvider serviceProvider)
        {
            var requestType = request.GetType();
            var factory = Cache.GetOrAdd(requestType, type => CreateFactory<TResponse>(type, serviceProvider));
            return (RequestPipeline<TResponse>)factory();
        }

        private static Func<object> CreateFactory<TResponse>(Type requestType, IServiceProvider serviceProvider)
        {
            var responseType = typeof(TResponse);
            var factoryBuilderType = typeof(FactoryBuilder<,>).MakeGenericType(requestType, responseType);
            var factoryBuilder = (FactoryBuilder<TResponse>)Activator.CreateInstance(factoryBuilderType);
            return factoryBuilder.CreateFactory(serviceProvider);
        }

        private abstract class FactoryBuilder<TResponse>
        {
            internal abstract Func<RequestPipeline<TResponse>> CreateFactory(IServiceProvider serviceProvider);
        }

        private class FactoryBuilder<TRequest, TResponse> : FactoryBuilder<TResponse>
            where TRequest : IRequest<TResponse>
        {
            internal override Func<RequestPipeline<TResponse>> CreateFactory(IServiceProvider serviceProvider)
            {
                var attributes = typeof(TRequest).GetCustomAttributes<RequestPipelineBuilderAttribute>().ToListOptimized();

                if (attributes.Count == 0)
                {
                    var defaultPipeline = new DefaultRequestPipeline<TRequest, TResponse>();
                    return () => defaultPipeline;
                }

                if (attributes.All(x => x.PipelineCanBeReused))
                {
                    var customizedPipeline = CreateCustomizedRequestPipeline();
                    return () => customizedPipeline;
                }

                return CreateCustomizedRequestPipeline;

                CustomizedRequestPipeline<TRequest, TResponse> CreateCustomizedRequestPipeline()
                {
                    var builder = new RequestPipelineBuilder<TRequest, TResponse>();

                    foreach (var attribute in attributes)
                    {
                        attribute.Configure(builder, serviceProvider);
                    }

                    return new CustomizedRequestPipeline<TRequest, TResponse>
                    {
                        FilterSelectors = builder.FilterSelectors,
                        HandlerSelectors = builder.HandlerSelectors,
                        HandlerExecutionStrategy = builder.HandlerExecutionStrategy ??
                                                   DefaultRequestHandlerExecutionStrategy<TRequest, TResponse>.Instance
                    };
                }
            }
        }
    }
}