using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handyman.Mediator.Pipeline
{
    internal class RequestPipelineFactory
    {
        private readonly ConcurrentDictionary<Type, IPipelineFactory> _factories = new ConcurrentDictionary<Type, IPipelineFactory>();
        private readonly ConcurrentDictionary<Type, Func<IServiceProvider, object>> _factoryMethods = new ConcurrentDictionary<Type, Func<IServiceProvider, object>>();

        internal RequestPipeline<TResponse> GetPipeline<TResponse>(IRequest<TResponse> request, MediatorOptions options, IServiceProvider serviceProvider)
        {
            var requestType = request.GetType();
            var responseType = typeof(TResponse);
            var factory = _factories.GetOrAdd(requestType, _ => CreateFactory(requestType, responseType));
            return (RequestPipeline<TResponse>)factory.CreatePipeline(options, serviceProvider);
        }

        private static IPipelineFactory CreateFactory(Type requestType, Type responseType)
        {
            var pipelineFactoryType = typeof(PipelineFactory<,>).MakeGenericType(requestType, responseType);
            return (IPipelineFactory)Activator.CreateInstance(pipelineFactoryType);
        }

        private interface IPipelineFactory
        {
            public abstract object CreatePipeline(MediatorOptions options, IServiceProvider serviceProvider);
        }

        private class PipelineFactory<TRequest, TResponse> : IPipelineFactory
            where TRequest : IRequest<TResponse>
        {
            private readonly List<IRequestPipelineBuilder> AttributePipelineBuilders = typeof(TRequest).GetCustomAttributes<RequestPipelineBuilderAttribute>()
                .Cast<IRequestPipelineBuilder>()
                .ToListOptimized();
            private readonly RequestPipeline<TRequest, TResponse> DefaultPipeline = new DefaultRequestPipeline<TRequest, TResponse>();

            public object CreatePipeline(MediatorOptions options, IServiceProvider serviceProvider)
            {
                var noPipelineBuilders = AttributePipelineBuilders.Count == 0 && options.EventPipelineBuilders.Count == 0;

                if (noPipelineBuilders)
                {
                    return DefaultPipeline;
                }

                List<IRequestPipelineBuilder>? pipelineBuilders = null;

                if (AttributePipelineBuilders.Count == 0)
                {
                    pipelineBuilders = options.RequestPipelineBuilders;
                }
                else if (options.RequestPipelineBuilders.Count == 0)
                {
                    pipelineBuilders = AttributePipelineBuilders;
                }
                else
                {
                    pipelineBuilders = new List<IRequestPipelineBuilder>();
                    pipelineBuilders.AddRange(AttributePipelineBuilders);
                    pipelineBuilders.AddRange(options.RequestPipelineBuilders);
                }

                pipelineBuilders.Sort(PipelineBuilderComparer.Compare);

                return new CustomizedRequestPipeline<TRequest, TResponse>()
                {
                    PipelineBuilders = pipelineBuilders
                };
            }
        }
    }
}