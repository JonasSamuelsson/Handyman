using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventDispatcherTests
    {
        [Fact]
        public async Task ShouldPublishEvent()
        {
            var services = new ServiceCollection()
                .AddTransient<IMediator>(x => new Mediator(x))
                .AddTransient<IEventHandler<Event>, Handler>()
                .AddTransient(typeof(IEventDispatcher<>), typeof(EventDispatcher<>))
                .BuildServiceProvider();

            var @event = new Event();

            await services.GetRequiredService<IEventDispatcher<Event>>().Publish(@event, CancellationToken.None);

            @event.Text.ShouldBe("success");
        }

        private class Event : IEvent
        {
            public string Text { get; set; }
        }

        private class Handler : IEventHandler<Event>
        {
            public Task Handle(Event @event, CancellationToken cancellationToken)
            {
                @event.Text = "success";
                return Task.CompletedTask;
            }
        }
    }
}