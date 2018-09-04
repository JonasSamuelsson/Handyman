using Maestro;
using Shouldly;
using System.Linq;
using System.Threading;
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
            var container = new Container();
            var eventHandler1 = new EventHandler();
            var eventHandler2 = new EventHandler();
            container.Configure(x => x.Add<IEventHandler<Event>>().Factory(() => eventHandler1));
            container.Configure(x => x.Add<IEventHandler<Event>>().Factory(() => eventHandler2));
            var mediator = new Mediator(container.GetService);

            await Task.WhenAll(mediator.Publish(@event, CancellationToken.None).ToArray());

            eventHandler1.Event.ShouldBe(@event);
            eventHandler2.Event.ShouldBe(@event);
        }

        private class Event : IEvent { }

        private class EventHandler : IEventHandler<Event>
        {
            public Event Event { get; set; }

            public Task Handle(Event @event, CancellationToken cancellationToken)
            {
                Event = @event;
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task ShouldPublishAsyncEvent()
        {
            var @event = new Event();
            var container = new Container();
            var eventHandler = new EventHandler();
            var synchronousEventHandler = new SynchEventHandler();
            container.Configure(x => x.Add<IEventHandler<Event>>().Factory(() => eventHandler));
            container.Configure(x => x.Add<IEventHandler<Event>>().Factory(() => synchronousEventHandler));
            var mediator = new Mediator(container.GetService);

            await Task.WhenAll(mediator.Publish(@event, CancellationToken.None));

            eventHandler.Event.ShouldBe(@event);
            synchronousEventHandler.Event.ShouldBe(@event);
        }

        private class SynchEventHandler : SynchEventHandler<Event>
        {
            public Event Event { get; set; }

            protected override void Handle(Event @event)
            {
                Event = @event;
            }
        }
    }
}