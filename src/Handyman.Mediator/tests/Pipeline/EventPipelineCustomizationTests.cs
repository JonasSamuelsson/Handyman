using Handyman.Mediator.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests.Pipeline
{
    public class EventPipelineCustomizationTests
    {
        [Fact]
        public async Task ShouldCustomizeFilterExecutionOrder()
        {
            var strings = new List<string>();

            var services = new ServiceCollection();

            services.AddSingleton<Action<string>>(s => strings.Add(s));

            var mediator = new Mediator(services.BuildServiceProvider());

            await mediator.Publish(new Event());

            strings.ShouldBe(new[] { "filter", "execution strategy", "handler" });
        }

        [CustomizeEventPipeline]
        private class Event : IEvent { }

        private class CustomizeEventPipelineAttribute : EventPipelineBuilderAttribute
        {
            public override void Configure(IEventPipelineBuilder builder, IServiceProvider serviceProvider)
            {
                builder.AddFilterSelector(new EventFilterSelector());
                builder.AddHandlerSelector(new EventHandlerSelector());
                builder.UseHandlerExecutionStrategy(new EventHandlerExecutionStrategy { Action = serviceProvider.GetRequiredService<Action<string>>() });
            }
        }

        private class EventFilterSelector : IEventFilterSelector
        {
            public Task SelectFilters<TEvent>(List<IEventFilter<TEvent>> filters, EventPipelineContext<TEvent> context)
                where TEvent : IEvent
            {
                filters.Add(new EventFilter<TEvent> { Action = context.ServiceProvider.GetRequiredService<Action<string>>() });
                return Task.CompletedTask;
            }
        }

        private class EventHandlerSelector : IEventHandlerSelector
        {
            public Task SelectHandlers<TEvent>(List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context) where TEvent : IEvent
            {
                handlers.Add(new EventHandler<TEvent> { Action = context.ServiceProvider.GetRequiredService<Action<string>>() });
                return Task.CompletedTask;
            }
        }

        private class EventFilter<TEvent> : IEventFilter<TEvent>
        {
            public Action<string> Action { get; set; }

            public Task Execute(EventPipelineContext<TEvent> pipelineContext, EventFilterExecutionDelegate next)
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

        private class EventHandlerExecutionStrategy : IEventHandlerExecutionStrategy
        {
            public Action<string> Action { get; set; }

            public Task Execute<TEvent>(List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context) where TEvent : IEvent
            {
                Action("execution strategy");
                return WhenAllEventHandlerExecutionStrategy.Instance.Execute(handlers, context);
            }
        }
    }
}
