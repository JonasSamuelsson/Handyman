using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventHandlerTests
    {
        [Fact]
        public async Task SyncImplementation()
        {
            var handler = new EventHandler();
            var services = new ServiceCollection().AddSingleton<IEventHandler<Event>>(handler);
            var mediator = new Mediator(services.BuildServiceProvider());

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