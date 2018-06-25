using Shouldly;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class PublishEventTests
    {
        [Fact]
        public async Task ShouldPublishEvent()
        {
            var @event = new Event();
            var serviceProvider = new ServiceProvider();
            serviceProvider.Add<IEventHandler<Event>, EventHandler1>();
            serviceProvider.Add<IEventHandler<Event>, EventHandler2>();
            var mediator = new Mediator(serviceProvider);

            await Task.WhenAll(mediator.Publish(@event).ToArray());

            @event.HandlerTypes.Count.ShouldBe(2);
            @event.HandlerTypes.ShouldContain(typeof(EventHandler1));
            @event.HandlerTypes.ShouldContain(typeof(EventHandler2));
        }

        class Event : IEvent
        {
            public ConcurrentBag<Type> HandlerTypes { get; } = new ConcurrentBag<Type>();
        }

        class EventHandler : IEventHandler<Event>
        {
            public Task Handle(Event @event)
            {
                @event.HandlerTypes.Add(GetType());
                return Task.CompletedTask;
            }
        }

        class EventHandler1 : EventHandler { }
        class EventHandler2 : EventHandler { }

        [Fact]
        public async Task ShouldPublishAsyncEvent()
        {
            var @event = new Event();
            var serviceProvider = new ServiceProvider();
            serviceProvider.Add<IEventHandler<Event>, EventHandler>();
            serviceProvider.Add<IEventHandler<Event>, SynchronousEventHandler>();
            var mediator = new Handyman.Mediator.Mediator(serviceProvider);

            await Task.WhenAll(mediator.Publish(@event));

            @event.HandlerTypes.Count.ShouldBe(2);
            @event.HandlerTypes.ShouldContain(typeof(EventHandler));
            @event.HandlerTypes.ShouldContain(typeof(SynchronousEventHandler));
        }

        class SynchronousEventHandler : SynchronousEventHandler<Event>
        {
            protected override void Handle(Event @event)
            {
                @event.HandlerTypes.Add(GetType());
            }
        }
    }
}