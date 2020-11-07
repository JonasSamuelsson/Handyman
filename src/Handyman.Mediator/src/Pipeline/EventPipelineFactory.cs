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

        public EventPipeline GetPipeline(IEvent @event, IServiceProvider serviceProvider, MediatorOptions options)
        {
            var eventType = @event.GetType();
            var factory = _factories.GetOrAdd(eventType, CreatePipelineFactory);
            return factory.CreatePipeline(options, serviceProvider);
        }

        private static PipelineFactory CreatePipelineFactory(Type eventType)
        {
            var pipelineFactoryType = typeof(PipelineFactory<>).MakeGenericType(eventType);
            return (PipelineFactory)Activator.CreateInstance(pipelineFactoryType);
        }

        private abstract class PipelineFactory
        {
            internal abstract EventPipeline CreatePipeline(MediatorOptions options, IServiceProvider serviceProvider);
        }

        private class PipelineFactory<TEvent> : PipelineFactory where TEvent : IEvent
        {
            private readonly List<IEventPipelineBuilder> AttributeBuilders = typeof(TEvent).GetCustomAttributes<EventPipelineBuilderAttribute>()
                .Cast<IEventPipelineBuilder>()
                .ToListOptimized();
            private readonly EventPipeline DefaultPipeline = new DefaultEventPipeline<TEvent>(MediatorDefaults.EventHandlerExecutionStrategy);

            internal override EventPipeline CreatePipeline(MediatorOptions options, IServiceProvider serviceProvider)
            {
                var noPipelineBuilders = AttributeBuilders.Count == 0 && options.EventPipelineBuilders.Count == 0;

                if (noPipelineBuilders)
                {
                    return options.EventHandlerExecutionStrategy != null
                        ? new DefaultEventPipeline<TEvent>(options.EventHandlerExecutionStrategy)
                        : DefaultPipeline;
                }

                var handlerExecutionStrategy = options.EventHandlerExecutionStrategy ??
                                               MediatorDefaults.EventHandlerExecutionStrategy;

                List<IEventPipelineBuilder>? pipelineBuilders = null;

                if (AttributeBuilders.Count == 0)
                {
                    pipelineBuilders = options.EventPipelineBuilders;
                }
                else if (options.EventPipelineBuilders.Count == 0)
                {
                    pipelineBuilders = AttributeBuilders;
                }
                else
                {
                    pipelineBuilders = new List<IEventPipelineBuilder>();
                    pipelineBuilders.AddRange(AttributeBuilders);
                    pipelineBuilders.AddRange(options.EventPipelineBuilders);
                }

                pipelineBuilders.Sort(PipelineBuilderComparer.Compare);

                return new CustomizedEventPipeline<TEvent>
                {
                    HandlerExecutionStrategy = handlerExecutionStrategy,
                    PipelineBuilders = pipelineBuilders
                };
            }
        }
    }
}