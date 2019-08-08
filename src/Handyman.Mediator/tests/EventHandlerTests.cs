using Maestro;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventHandlerTests
    {
        [Fact]
        public async Task SyncImplementation()
        {
            var handler = new EventHandler();
            var container = new Container(x => x.Add<IEventHandler<Event>>().Instance(handler));
            var mediator = new Mediator(container.GetService);

            await mediator.Publish(new Event());
        }

        private class Event : IEvent { }

        private class EventHandler : EventHandler<Event>
        {
            protected override void Handle(Event @event)
            {
            }
        }
    }
}