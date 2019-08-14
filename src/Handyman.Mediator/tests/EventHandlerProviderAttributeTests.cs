using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Maestro;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventHandlerProviderAttributeTests
    {
        [Fact]
        public async Task ShouldGetEventHandlerViaAttribute()
        {
            var container = new Container();

            var mediator = new Mediator(container.GetService);

            var @event = new Event();

            await mediator.Publish(@event);

            @event.Strings.ShouldBe(new[] { "success" });
        }

        [CustomEventHandlerProvider]
        public class Event : IEvent
        {
            public List<string> Strings { get; set; } = new List<string>();
        }

        private class CustomEventHandlerProviderAttribute : EventHandlerProviderAttribute
        {
            public override IEnumerable<IEventHandler<TEvent>> GetHandlers<TEvent>(ServiceProvider serviceProvider)
            {
                return new[] { new EventHandler<TEvent>() };
            }
        }

        private class EventHandler<TEvent> : IEventHandler<TEvent> where TEvent : IEvent
        {
            public Task Handle(TEvent @event, CancellationToken cancellationToken)
            {
                ((Event)(object)@event).Strings.Add("success");
                return Task.CompletedTask;
            }
        }
    }
}