﻿using Handyman.Mediator.Pipeline;
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

            var builder = new EventPipelineBuilder();

            var mediator = new Mediator(services.BuildServiceProvider(), new MediatorOptions { EventPipelineBuilders = { builder } });

            await mediator.Publish(new Event());

            strings.ShouldBe(new[] { "filter", "execution strategy", "handler" });

            builder.Executed.ShouldBeTrue();
        }

        [CustomizeEventPipeline]
        private class Event : IEvent { }

        private class CustomizeEventPipelineAttribute : EventPipelineBuilderAttribute
        {
            public override Task Execute<TEvent>(EventPipelineBuilderContext<TEvent> pipelineBuilderContext, EventContext<TEvent> eventContext)
            {
                pipelineBuilderContext.Filters.Add(new EventFilter<TEvent> { Action = eventContext.ServiceProvider.GetRequiredService<Action<string>>() });
                pipelineBuilderContext.Handlers.Add(new EventHandler<TEvent> { Action = eventContext.ServiceProvider.GetRequiredService<Action<string>>() });
                pipelineBuilderContext.HandlerExecutionStrategy = new EventHandlerExecutionStrategy { Action = eventContext.ServiceProvider.GetRequiredService<Action<string>>() };

                return Task.CompletedTask;
            }
        }

        private class EventPipelineBuilder : IEventPipelineBuilder
        {
            public bool Executed { get; set; }

            public Task Execute<TEvent>(EventPipelineBuilderContext<TEvent> pipelineBuilderContext, EventContext<TEvent> eventContext) where TEvent : IEvent
            {
                Executed = true;
                return Task.CompletedTask;
            }
        }

        private class EventFilter<TEvent> : IEventFilter<TEvent>
        {
            public Action<string> Action { get; set; }

            public Task Execute(EventContext<TEvent> eventContext, EventFilterExecutionDelegate next)
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

            public Task Execute<TEvent>(List<IEventHandler<TEvent>> handlers, EventContext<TEvent> eventContext) where TEvent : IEvent
            {
                Action("execution strategy");
                return WhenAllEventHandlerExecutionStrategy.Instance.Execute(handlers, eventContext);
            }
        }
    }
}
