using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handyman.Mediator.Pipeline
{
    internal class EventPipelineFactory
    {
        private readonly ConcurrentDictionary<Type, PipelineFactory> _factories = new ConcurrentDictionary<Type, PipelineFactory>();

        public EventPipeline GetPipeline(IEvent @event, MediatorOptions mediatorOptions, IServiceProvider serviceProvider)
        {
            var eventType = @event.GetType();
            var factory = _factories.GetOrAdd(eventType, CreatePipelineFactory);
            return factory.CreatePipeline(mediatorOptions, serviceProvider);
        }

        private static PipelineFactory CreatePipelineFactory(Type eventType)
        {
            var pipelineFactoryType = typeof(PipelineFactory<>).MakeGenericType(eventType);
            var instance = Activator.CreateInstance(pipelineFactoryType);
            return (PipelineFactory)(instance ?? throw new InvalidOperationException($"Can't create instance of {pipelineFactoryType.FullName}."));
        }

        private abstract class PipelineFactory
        {
            internal abstract EventPipeline CreatePipeline(MediatorOptions options, IServiceProvider serviceProvider);
        }

        private class PipelineFactory<TEvent> : PipelineFactory where TEvent : IEvent
        {
            private static readonly List<IEventPipelineBuilder> AttributePipelineBuilders = typeof(TEvent).GetCustomAttributes<EventPipelineBuilderAttribute>()
                .Cast<IEventPipelineBuilder>()
                .OrderBy(PipelineBuilderComparer.GetOrder)
                .ToListOptimized();

            private static readonly EventPipeline DefaultPipeline = new DefaultEventPipeline<TEvent>(MediatorDefaults.EventHandlerExecutionStrategy);

            internal override EventPipeline CreatePipeline(MediatorOptions options, IServiceProvider serviceProvider)
            {
                var noPipelineBuilders = AttributePipelineBuilders.Count == 0 && options.EventPipelineBuilders.Count == 0;

                if (noPipelineBuilders)
                {
                    return options.EventHandlerExecutionStrategy != null
                        ? new DefaultEventPipeline<TEvent>(options.EventHandlerExecutionStrategy)
                        : DefaultPipeline;
                }

                var handlerExecutionStrategy = options.EventHandlerExecutionStrategy ??
                                               MediatorDefaults.EventHandlerExecutionStrategy;

                List<IEventPipelineBuilder>? pipelineBuilders;

                if (options.EventPipelineBuilders.Count == 0)
                {
                    pipelineBuilders = AttributePipelineBuilders;
                }
                else if (AttributePipelineBuilders.Count == 0)
                {
                    pipelineBuilders = options.EventPipelineBuilders.ToList();
                    pipelineBuilders.Sort(PipelineBuilderComparer.Compare);
                }
                else
                {
                    pipelineBuilders = new List<IEventPipelineBuilder>();
                    pipelineBuilders.AddRange(AttributePipelineBuilders);
                    pipelineBuilders.AddRange(options.EventPipelineBuilders);
                    pipelineBuilders.Sort(PipelineBuilderComparer.Compare);
                }

                return new CustomizedEventPipeline<TEvent>
                {
                    HandlerExecutionStrategy = handlerExecutionStrategy,
                    PipelineBuilders = pipelineBuilders
                };
            }
        }
    }
}