using Handyman.Mediator.EventPipelineCustomization;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace Handyman.Mediator.Internals
{
    internal static class EventPipelineFactory
    {
        private static readonly ConcurrentDictionary<Type, Func<object>> Cache = new ConcurrentDictionary<Type, Func<object>>();

        public static EventPipeline GetPipeline(IEvent @event, IServiceProvider serviceProvider)
        {
            var eventType = @event.GetType();
            var factory = Cache.GetOrAdd(eventType, type => CreateFactory(type, serviceProvider));
            return (EventPipeline)factory();
        }

        private static Func<object> CreateFactory(Type eventType, IServiceProvider serviceProvider)
        {
            var factoryBuilderType = typeof(FactoryBuilder<>).MakeGenericType(eventType);
            var factoryBuilder = (FactoryBuilder)Activator.CreateInstance(factoryBuilderType);
            return factoryBuilder.CreateFactory(serviceProvider);
        }

        private abstract class FactoryBuilder
        {
            internal abstract Func<EventPipeline> CreateFactory(IServiceProvider serviceProvider);
        }

        private class FactoryBuilder<TEvent> : FactoryBuilder
            where TEvent : IEvent
        {
            internal override Func<EventPipeline> CreateFactory(IServiceProvider serviceProvider)
            {
                var attributes = typeof(TEvent).GetCustomAttributes<EventPipelineBuilderAttribute>().ToListOptimized();

                if (attributes.Count == 0)
                {
                    var defaultPipeline = new DefaultEventPipeline<TEvent>();
                    return () => defaultPipeline;
                }

                if (attributes.All(x => x.PipelineCanBeReused))
                {
                    var customizedPipeline = CreateCustomizedEventPipeline();
                    return () => customizedPipeline;
                }

                return CreateCustomizedEventPipeline;

                CustomizedEventPipeline<TEvent> CreateCustomizedEventPipeline()
                {
                    var builder = new EventPipelineBuilder<TEvent>();

                    foreach (var attribute in attributes)
                    {
                        attribute.Configure<TEvent>(builder, serviceProvider);
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