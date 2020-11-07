using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Handyman.Mediator.Pipeline
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
                var builderAttributes = typeof(TRequest).GetCustomAttributes<RequestPipelineBuilderAttribute>()
                    .Cast<IRequestPipelineBuilder>()
                    .ToListOptimized();

                if (builderAttributes.Count == 0)
                {
                    return _ => DefaultRequestPipeline<TRequest, TResponse>.Instance;
                }

                return CreateCustomizedPipeline;

                object CreateCustomizedPipeline(IServiceProvider serviceProvider)
                {
                    if (builderAttributes.Count != 1)
                    {
                        builderAttributes.Sort(PipelineBuilderComparer.Compare);
                    }

                    return new CustomizedRequestPipeline<TRequest, TResponse>
                    {
                        PipelineBuilders = builderAttributes
                    };
                }
            }
        }
    }
}