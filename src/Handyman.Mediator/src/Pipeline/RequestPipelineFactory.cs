using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handyman.Mediator.Pipeline
{
    internal class RequestPipelineFactory
    {
        private readonly ConcurrentDictionary<Type, PipelineFactory> _factories = new ConcurrentDictionary<Type, PipelineFactory>();

        internal RequestPipeline<TResponse> GetPipeline<TResponse>(IRequest<TResponse> request, MediatorOptions options, IServiceProvider serviceProvider)
        {
            var requestType = request.GetType();
            var responseType = typeof(TResponse);
            var factory = _factories.GetOrAdd(requestType, _ => CreateFactory(requestType, responseType));
            return (RequestPipeline<TResponse>)factory.CreatePipeline(options, serviceProvider);
        }

        private static PipelineFactory CreateFactory(Type requestType, Type responseType)
        {
            var pipelineFactoryType = typeof(PipelineFactory<,>).MakeGenericType(requestType, responseType);
            return (PipelineFactory)Activator.CreateInstance(pipelineFactoryType);
        }

        private abstract class PipelineFactory
        {
            internal abstract object CreatePipeline(MediatorOptions options, IServiceProvider serviceProvider);
        }

        private class PipelineFactory<TRequest, TResponse> : PipelineFactory
            where TRequest : IRequest<TResponse>
        {
            private static readonly List<IRequestPipelineBuilder> AttributePipelineBuilders = typeof(TRequest).GetCustomAttributes<RequestPipelineBuilderAttribute>()
                .Cast<IRequestPipelineBuilder>()
                .OrderBy(PipelineBuilderComparer.GetOrder)
                .ToListOptimized();
            private static readonly RequestPipeline<TRequest, TResponse> DefaultPipeline = new DefaultRequestPipeline<TRequest, TResponse>();

            internal override object CreatePipeline(MediatorOptions options, IServiceProvider serviceProvider)
            {
                var noPipelineBuilders = AttributePipelineBuilders.Count == 0 && options.RequestPipelineBuilders.Count == 0;

                if (noPipelineBuilders)
                {
                    return DefaultPipeline;
                }

                List<IRequestPipelineBuilder>? pipelineBuilders;

                if (options.RequestPipelineBuilders.Count == 0)
                {
                    pipelineBuilders = AttributePipelineBuilders;
                }
                else if (AttributePipelineBuilders.Count == 0)
                {
                    pipelineBuilders = options.RequestPipelineBuilders.ToList();
                    pipelineBuilders.Sort(PipelineBuilderComparer.Compare);
                }
                else
                {
                    pipelineBuilders = new List<IRequestPipelineBuilder>();
                    pipelineBuilders.AddRange(AttributePipelineBuilders);
                    pipelineBuilders.AddRange(options.RequestPipelineBuilders);
                    pipelineBuilders.Sort(PipelineBuilderComparer.Compare);
                }

                return new CustomizedRequestPipeline<TRequest, TResponse>()
                {
                    PipelineBuilders = pipelineBuilders
                };
            }
        }
    }
}