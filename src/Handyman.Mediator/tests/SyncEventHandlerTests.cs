using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class SyncEventHandlerTests
    {
        [Fact]
        public async Task SyncImplementation()
        {
            var handler = new EventHandler();
            var services = new ServiceCollection().AddSingleton<IEventHandler<Event>>(handler);
            var mediator = new Mediator(services.BuildServiceProvider());

            await mediator.Publish(new Event());
        }

        private class Event : IEvent
        {
        }

        private class EventHandler : SyncEventHandler<Event>
        {
            public override void Handle(Event @event, CancellationToken cancellationToken)
            {
            }
        }
    }
}