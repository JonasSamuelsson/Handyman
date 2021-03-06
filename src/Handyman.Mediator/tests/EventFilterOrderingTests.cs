﻿using Handyman.Mediator.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventFilterOrderingTests
    {
        [Fact]
        public async Task FiltersShouldBeExecutedInTheCorrectOrder()
        {
            var services = new ServiceCollection();

            services.AddSingleton<IEventFilter<Event>>(new Filter { Text = "b" });
            services.AddSingleton<IEventFilter<Event>>(new Filter { Order = 1, Text = "c" });
            services.AddSingleton<IEventFilter<Event>>(new Filter { Order = -1, Text = "a" });
            services.AddTransient<IEventHandler<Event>, Handler>();

            var @event = new Event();

            await new Mediator(services.BuildServiceProvider())
                .Publish(@event);

            @event.Text.ShouldBe("abc");
        }

        private class Event : IEvent
        {
            public string Text { get; set; }
        }

        private class Filter : IOrderedFilter, IEventFilter<Event>
        {
            public int Order { get; set; }
            public string Text { get; set; }

            public Task Execute(EventContext<Event> eventContext, EventFilterExecutionDelegate next)
            {
                eventContext.Event.Text += Text;
                return next();
            }
        }

        private class Handler : IEventHandler<Event>
        {
            public Task Handle(Event @event, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}