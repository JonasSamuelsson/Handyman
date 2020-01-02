using Handyman.Mediator.EventPipelineCustomization;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Handyman.Mediator.Internals
{
    internal class EventPipelineFactory
    {
        private readonly ConcurrentDictionary<Type, Func<EventPipeline>> _factoryMethods = new ConcurrentDictionary<Type, Func<EventPipeline>>();

        public EventPipeline GetPipeline(IEvent @event, IServiceProvider serviceProvider)
        {
            var eventType = @event.GetType();
            var factoryMethod = _factoryMethods.GetOrAdd(eventType, type => CreateFactoryMethod(type, serviceProvider));
            return factoryMethod.Invoke();
        }

        private static Func<EventPipeline> CreateFactoryMethod(Type eventType, IServiceProvider serviceProvider)
        {
            var factoryMethodBuilderType = typeof(FactoryMethodBuilder<>).MakeGenericType(eventType);
            var factoryMethodBuilder = (FactoryMethodBuilder)Activator.CreateInstance(factoryMethodBuilderType);
            return factoryMethodBuilder.CreateFactoryMethod(serviceProvider);
        }

        private abstract class FactoryMethodBuilder
        {
            internal abstract Func<EventPipeline> CreateFactoryMethod(IServiceProvider serviceProvider);
        }

        private class FactoryMethodBuilder<TEvent> : FactoryMethodBuilder
            where TEvent : IEvent
        {
            internal override Func<EventPipeline> CreateFactoryMethod(IServiceProvider serviceProvider)
            {
                var attributes = typeof(TEvent).GetCustomAttributes<EventPipelineBuilderAttribute>().ToListOptimized();

                if (attributes.Count == 0)
                {
                    return () => DefaultEventPipeline<TEvent>.Instance;
                }

                return CreateCustomizedEventPipeline;

                CustomizedEventPipeline<TEvent> CreateCustomizedEventPipeline()
                {
                    var builder = new EventPipelineBuilder();

                    foreach (var attribute in attributes)
                    {
                        attribute.Configure(builder, serviceProvider);
                    }

                    return new CustomizedEventPipeline<TEvent>
                    {
                        FilterSelectors = builder.FilterSelectors,
                        HandlerSelectors = builder.HandlerSelectors,
                        HandlerExecutionStrategy = builder.HandlerExecutionStrategy ?? DefaultEventHandlerExecutionStrategy.Instance
                    };
                }
            }
        }
    }
}