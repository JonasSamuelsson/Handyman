using Handyman.Mediator.Internals;
using Maestro;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventFilterOrderingTests
    {
        [Fact]
        public async Task FiltersShouldBeExecutedInTheCorrectOrder()
        {
            var container = new Container(x =>
            {
                x.Add<IEventFilter<Event>>().Instance(new Filter { Text = "b" });
                x.Add<IEventFilter<Event>>().Instance(new Filter { Order = 1, Text = "c" });
                x.Add<IEventFilter<Event>>().Instance(new Filter { Order = -1, Text = "a" });
                x.Add<IEventHandler<Event>>().Type<Handler>();
            });

            var @event = new Event();

            await new Mediator(container.GetService, new Configuration { EventFilterProvider = DefaultEventFilterProvider.Instance })
                .Publish(@event);

            @event.Text.ShouldBe("abc");
        }

        private class Event : IEvent
        {
            public string Text { get; set; }
        }

        private class Filter : IOrderedFilter, IEventFilter<Event>
        {
            public int Order { get; set; }
            public string Text { get; set; }

            public Task Execute(EventFilterContext<Event> context, EventFilterExecutionDelegate next)
            {
                context.Event.Text += Text;
                return next();
            }
        }

        private class Handler : IEventHandler<Event>
        {
            public Task Handle(Event @event, CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}