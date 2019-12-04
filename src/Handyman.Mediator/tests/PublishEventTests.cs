using Maestro;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
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

            var services = new ServiceCollection();

            services.AddSingleton<IEventHandler<Event>>(handler1);
            services.AddSingleton<IEventHandler<Event>>(handler2);

            var mediator = new Mediator(services.BuildServiceProvider());

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