using System.Threading;
using System.Threading.Tasks;
using Maestro;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class SyncEventHandlerTests
    {
        [Fact]
        public async Task UseSyncEventHandler()
        {
            var handler = new EventHandler();
            var container = new Container(x => x.Add<IEventHandler<Event>>().Instance(handler));
            var mediator = new Mediator(container.GetService);

            await Task.WhenAll(mediator.Publish(new Event(), CancellationToken.None));

            handler.Executed.ShouldBeTrue();
        }

        private class Event : IEvent { }

        private class EventHandler : SyncEventHandler<Event>
        {
            public bool Executed { get; set; }

            protected override void Handle(Event @event)
            {
                Executed = true;
            }
        }
    }
}