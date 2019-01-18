using Maestro;
using Shouldly;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventHandlerCustomizationTests
    {
        [Fact]
        public async Task ShouldCustomizeEventHandling()
        {
            var container = new Container(x =>
            {
                x.Add<IEventHandler<Event>>().Instance(new EventHandler { MillisecondsDelay = 0 });
                x.Add<IEventHandler<Event>>().Instance(new EventHandler { MillisecondsDelay = int.MaxValue });
            });

            var e = new Event();
            await new Mediator(container.GetService, new Configuration { EventHandlerProvider = new EventHandlerProvider() }).Publish(e);

            e.Tasks.Count(x => x.IsCompleted).ShouldBe(1);
            e.Tasks.Count(x => !x.IsCompleted).ShouldBe(1);
        }

        private class Event : IEvent
        {
            public List<Task> Tasks { get; set; } = new List<Task>();
        }

        private class EventHandler : IEventHandler<Event>
        {
            public int MillisecondsDelay { get; set; }

            public Task Handle(Event @event, CancellationToken cancellationToken)
            {
                var task = Task.Delay(MillisecondsDelay, cancellationToken);
                @event.Tasks.Add(task);
                return task;
            }
        }

        private class EventHandlerProvider : IEventHandlerProvider
        {
            public IEnumerable<IEventHandler<TEvent>> GetHandlers<TEvent>(ServiceProvider serviceProvider) where TEvent : IEvent
            {
                var type = typeof(IEnumerable<IEventHandler<TEvent>>);
                var handlers = (IEnumerable<IEventHandler<TEvent>>)serviceProvider.Invoke(type);
                return new IEventHandler<TEvent>[] { new EventHandler<TEvent>(handlers) };
            }

            private class EventHandler<T> : IEventHandler<T> where T : IEvent
            {
                private readonly IEnumerable<IEventHandler<T>> _handlers;

                public EventHandler(IEnumerable<IEventHandler<T>> handlers)
                {
                    _handlers = handlers;
                }

                public Task Handle(T @event, CancellationToken cancellationToken)
                {
                    return Task.WhenAny(_handlers.Select(x => x.Handle(@event, cancellationToken)));
                }
            }
        }
    }
}