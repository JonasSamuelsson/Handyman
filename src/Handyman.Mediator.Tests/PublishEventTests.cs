using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class PublishEventTests
    {
        [Fact]
        public void ShouldPublishEvent()
        {
            var @event = new Event();
            var serviceProvider = new ServiceProvider();
            serviceProvider.Add<IEventHandler<Event>, EventHandler1>();
            serviceProvider.Add<IEventHandler<Event>, EventHandler2>();
            var mediator = new Handyman.Mediator.Mediator(serviceProvider);

            mediator.Publish(@event);

            @event.HandlerTypes.Count.ShouldBe(2);
            @event.HandlerTypes.ShouldContain(typeof(EventHandler1));
            @event.HandlerTypes.ShouldContain(typeof(EventHandler2));
        }

        class Event : IEvent
        {
            public ConcurrentBag<Type> HandlerTypes { get; } = new ConcurrentBag<Type>();
        }

        abstract class EventHandler : IEventHandler<Event>
        {
            public void Handle(Event @event)
            {
                @event.HandlerTypes.Add(GetType());
            }
        }

        class EventHandler1 : EventHandler { }
        class EventHandler2 : EventHandler { }

        [Fact]
        public async Task ShouldPublishAsyncEvent()
        {
            var @event = new AsyncEvent();
            var serviceProvider = new ServiceProvider();
            serviceProvider.Add<IAsyncEventHandler<AsyncEvent>, AsyncEventHandler1>();
            serviceProvider.Add<IAsyncEventHandler<AsyncEvent>, AsyncEventHandler2>();
            var mediator = new Handyman.Mediator.Mediator(serviceProvider);

            await Task.WhenAll(mediator.Publish(@event));

            @event.HandlerTypes.Count.ShouldBe(2);
            @event.HandlerTypes.ShouldContain(typeof(AsyncEventHandler1));
            @event.HandlerTypes.ShouldContain(typeof(AsyncEventHandler2));
        }

        class AsyncEvent : IAsyncEvent
        {
            public ConcurrentBag<Type> HandlerTypes { get; } = new ConcurrentBag<Type>();
        }

        abstract class AsyncEventHandler : IAsyncEventHandler<AsyncEvent>
        {
            public Task Handle(AsyncEvent @event)
            {
                @event.HandlerTypes.Add(GetType());
                return Task.CompletedTask;
            }
        }

        class AsyncEventHandler1 : AsyncEventHandler { }
        class AsyncEventHandler2 : AsyncEventHandler { }
    }
}