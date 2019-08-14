using System.Collections.Generic;
using System.Threading.Tasks;
using Maestro;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventFilterProviderAttributeTests
    {
        [Fact]
        public async Task ShouldGetEventFilterViaAttribute()
        {
            var container = new Container();

            var mediator = new Mediator(container.GetService);

            var @event = new Event();

            await mediator.Publish(@event);

            @event.Strings.ShouldBe(new[] { "success" });
        }

        [CustomEventFilterProvider]
        public class Event : IEvent
        {
            public List<string> Strings { get; set; } = new List<string>();
        }

        private class CustomEventFilterProviderAttribute : EventFilterProviderAttribute
        {
            public override IEnumerable<IEventFilter<TEvent>> GetFilters<TEvent>(ServiceProvider serviceProvider)
            {
                return new[] { new EventFilter<TEvent>(), };
            }
        }

        private class EventFilter<TEvent> : IEventFilter<TEvent> where TEvent : IEvent
        {
            public Task Execute(EventFilterContext<TEvent> context, EventFilterExecutionDelegate next)
            {
                ((Event)(object)context.Event).Strings.Add("success");
                return Task.CompletedTask;
            }
        }
    }
}