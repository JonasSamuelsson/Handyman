using System;
using System.Collections.Concurrent;
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
            internal override Func<IServiceProvider, MediatorOptions, EventPipeline> CreateFactoryMethod()
            {
                var attributes = typeof(TEvent).GetCustomAttributes<EventPipelineBuilderAttribute>().ToListOptimized();

                if (attributes.Count == 0)
                {
                    return (serviceProvider, mediatorOptions) =>
                        mediatorOptions.EventHandlerExecutionStrategy == WhenAllEventHandlerExecutionStrategy.Instance
                            ? DefaultEventPipeline<TEvent>.DefaultInstance
                            : new DefaultEventPipeline<TEvent>(mediatorOptions.EventHandlerExecutionStrategy);
                }

                return CreateCustomizedEventPipeline;

                EventPipeline CreateCustomizedEventPipeline(IServiceProvider serviceProvider, MediatorOptions mediatorOptions)
                {
                    var builder = new EventPipelineBuilder();

                    if (attributes.Count != 1)
                    {
                        attributes.Sort((x, y) => x.ExecutionOrder.CompareTo(y.ExecutionOrder));
                    }

                    foreach (var attribute in attributes)
                    {
                        attribute.Configure(builder, serviceProvider);
                    }

                    return new CustomizedEventPipeline<TEvent>
                    {
                        FilterSelectors = builder.FilterSelectors,
                        HandlerSelectors = builder.HandlerSelectors,
                        HandlerExecutionStrategy = builder.HandlerExecutionStrategy ?? mediatorOptions.EventHandlerExecutionStrategy
                    };
                }
            }
        }
    }
}