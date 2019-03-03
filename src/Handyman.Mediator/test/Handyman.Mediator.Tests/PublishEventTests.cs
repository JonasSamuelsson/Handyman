using Maestro;
using Shouldly;
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
            var handler1 = new EventHandler();
            var handler2 = new EventHandler();
            var container = new Container(x =>
            {
                x.Add<IEventHandler<Event>>().Instance(handler1);
                x.Add<IEventHandler<Event>>().Instance(handler2);
            });
            var mediator = new Mediator(container.GetService);

            await Task.WhenAll(mediator.Publish(new Event(), CancellationToken.None));

            handler1.Executed.ShouldBeTrue();
            handler2.Executed.ShouldBeTrue();
        }

        private class Event : IEvent { }

        private class EventHandler : IEventHandler<Event>
        {
            public bool Executed { get; set; }

            public Task Handle(Event @event, CancellationToken cancellationToken)
            {
                Executed = true;
                return Task.CompletedTask;
            }
        }
    }
}