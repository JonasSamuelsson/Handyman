using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Handyman.Mediator.EventPipelineCustomization;
using Handyman.Mediator.Internals;
using Maestro;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests.EventPipelineCustomization
{
    public class EventPipelineCustomizationTests
    {
        [Fact]
        public async Task ShouldCustomizeFilterExecutionOrder()
        {
            var strings = new List<string>();

            var container = new Container(x => x.Add<Action<string>>().Instance(new Action<string>(s => strings.Add(s))));

            var mediator = new Mediator(container.GetService);

            await mediator.Publish(new Event());

            strings.ShouldBe(new[] { "filter", "execution strategy", "handler" });
        }

        [CustomizeEventPipeline]
        private class Event : IEvent { }

        private class CustomizeEventPipelineAttribute : EventPipelineBuilderAttribute
        {
            public override void Configure<TEvent>(IEventPipelineBuilder<TEvent> builder, ServiceProvider serviceProvider)
            {
                builder.AddFilterSelector(new EventFilterSelector<TEvent>());
                builder.AddHandlerSelector(new EventHandlerSelector<TEvent>());
                builder.UseHandlerExecutionStrategy(new EventHandlerExecutionStrategy<TEvent> { Action = serviceProvider.GetRequiredService<Action<string>>() });
            }
        }

        private class EventFilterSelector<TEvent> : IEventFilterSelector<TEvent>
            where TEvent : IEvent
        {
            public Task SelectFilters(List<IEventFilter<TEvent>> filters, EventPipelineContext<TEvent> context)
            {
                filters.Add(new EventFilter<TEvent> { Action = context.ServiceProvider.GetRequiredService<Action<string>>() });
                return Task.CompletedTask;
            }
        }

        private class EventHandlerSelector<TEvent> : IEventHandlerSelector<TEvent>
            where TEvent : IEvent
        {
            public Task SelectHandlers(List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context)
            {
                handlers.Add(new EventHandler<TEvent> { Action = context.ServiceProvider.GetRequiredService<Action<string>>() });
                return Task.CompletedTask;
            }
        }

        private class EventFilter<TEvent> : IEventFilter<TEvent>
            where TEvent : IEvent
        {
            public Action<string> Action { get; set; }

            public Task Execute(EventPipelineContext<TEvent> context, EventFilterExecutionDelegate next)
            {
                Action("filter");
                return next();
            }
        }

        private class EventHandler<TEvent> : IEventHandler<TEvent>
            where TEvent : IEvent
        {
            public Action<string> Action { get; set; }

            public Task Handle(TEvent @event, CancellationToken cancellationToken)
            {
                Action("handler");
                return Task.CompletedTask;
            }
        }

        private class EventHandlerExecutionStrategy<TEvent> : IEventHandlerExecutionStrategy<TEvent>
            where TEvent : IEvent
        {
            public Action<string> Action { get; set; }

            public Task Execute(List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context)
            {
                Action("execution strategy");
                return DefaultEventHandlerExecutionStrategy<TEvent>.Instance.Execute(handlers, context);
            }
        }
    }
}
