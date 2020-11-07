using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Handyman.Mediator.Pipeline
{
    internal class EventPipelineFactory
    {
        private readonly ConcurrentDictionary<Type, Func<IServiceProvider, MediatorOptions, EventPipeline>> _factoryMethods = new ConcurrentDictionary<Type, Func<IServiceProvider, MediatorOptions, EventPipeline>>();

        public EventPipeline GetPipeline(IEvent @event, IServiceProvider serviceProvider, MediatorOptions options)
        {
            var eventType = @event.GetType();
            var factoryMethod = _factoryMethods.GetOrAdd(eventType, CreateFactoryMethod);
            return factoryMethod.Invoke(serviceProvider, options);
        }

        private static Func<IServiceProvider, MediatorOptions, EventPipeline> CreateFactoryMethod(Type eventType)
        {
            var factoryMethodBuilderType = typeof(FactoryMethodBuilder<>).MakeGenericType(eventType);
            var factoryMethodBuilder = (FactoryMethodBuilder)Activator.CreateInstance(factoryMethodBuilderType);
            return factoryMethodBuilder.CreateFactoryMethod();
        }

        private abstract class FactoryMethodBuilder
        {
            internal abstract Func<IServiceProvider, MediatorOptions, EventPipeline> CreateFactoryMethod();
        }

        private class FactoryMethodBuilder<TEvent> : FactoryMethodBuilder
            where TEvent : IEvent
        {
            private static readonly EventPipeline DefaultPipeline = new DefaultEventPipeline<TEvent>(Defaults.EventHandlerExecutionStrategy);

            internal override Func<IServiceProvider, MediatorOptions, EventPipeline> CreateFactoryMethod()
            {
                var builderAttributes = typeof(TEvent).GetCustomAttributes<EventPipelineBuilderAttribute>()
                    .Cast<IEventPipelineBuilder>()
                    .ToListOptimized();

                if (builderAttributes.Count == 0)
                {
                    return (serviceProvider, mediatorOptions) =>
                    {
                        if (mediatorOptions.EventHandlerExecutionStrategy == null)
                        {
                            return DefaultPipeline;
                        }

                        return new DefaultEventPipeline<TEvent>(mediatorOptions.EventHandlerExecutionStrategy);
                    };
                }

                return CreateCustomizedEventPipeline;

                EventPipeline CreateCustomizedEventPipeline(IServiceProvider serviceProvider, MediatorOptions mediatorOptions)
                {
                    if (builderAttributes.Count != 1)
                    {
                        builderAttributes.Sort(PipelineBuilderComparer.Compare);
                    }

                    return new CustomizedEventPipeline<TEvent>
                    {
                        HandlerExecutionStrategy = mediatorOptions.EventHandlerExecutionStrategy,
                        PipelineBuilders = builderAttributes
                    };
                }
            }
        }
    }
}