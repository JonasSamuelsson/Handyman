using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class PublishEventTests
    {
        [Fact]
        public async Task ShouldPublishEventUsingMediator()
        {
            var mediator = new ServiceCollection()
                .AddTransient<IMediator>(sp => new Mediator(sp))
                .AddTransient<IEventHandler<Event>, EventHandler>()
                .BuildServiceProvider()
                .GetRequiredService<IMediator>();

            var @event = new Event();

            await mediator.Publish(@event, CancellationToken.None);

            @event.Handled.ShouldBeTrue();
        }

        [Fact]
        public async Task ShouldPublishEventUsingNonGenericPublisher()
        {
            var publisher = new ServiceCollection()
                .AddTransient<IPublisher>(sp => sp.GetRequiredService<IMediator>())
                .AddTransient<IMediator>(sp => new Mediator(sp))
                .AddTransient<IEventHandler<Event>, EventHandler>()
                .BuildServiceProvider()
                .GetRequiredService<IPublisher>();

            var @event = new Event();

            await publisher.Publish(@event, CancellationToken.None);

            @event.Handled.ShouldBeTrue();
        }

        [Fact]
        public async Task ShouldPublishEventUsingGenericPublisher()
        {
            var publisher = new ServiceCollection()
                .AddTransient(typeof(IPublisher<>), typeof(Publisher<>))
                .AddTransient<IMediator>(sp => new Mediator(sp))
                .AddTransient<IEventHandler<Event>, EventHandler>()
                .BuildServiceProvider()
                .GetRequiredService<IPublisher<Event>>();

            var @event = new Event();

            await publisher.Publish(@event, CancellationToken.None);

            @event.Handled.ShouldBeTrue();
        }

        private class Event : IEvent
        {
            public bool Handled { get; set; }
        }

        private class EventHandler : IEventHandler<Event>
        {
            public Task Handle(Event @event, CancellationToken cancellationToken)
            {
                @event.Handled = true;
                return Task.CompletedTask;
            }
        }
    }
}