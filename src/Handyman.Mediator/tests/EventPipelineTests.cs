using Maestro;
using Shouldly;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventPipelineTests
    {
        [Fact]
        public async Task ShouldNotExecuteFiltersIfPipelineIsDisabled()
        {
            var filter = new EventFilter();
            var handler = new EventHandler();

            var container = new Container(x =>
            {
                x.Add<IEventFilter<Event>>().Instance(filter);
                x.Add<IEventHandler<Event>>().Instance(handler);
            });

            var mediator = new Mediator(container.GetService);

            await Task.WhenAll(mediator.Publish(new Event()));

            filter.Executed.ShouldBeFalse();
            handler.Executed.ShouldBeTrue();
        }

        [Fact]
        public async Task ShouldExecuteEventFilters()
        {
            var filter = new EventFilter();
            var handler = new EventHandler();

            var container = new Container(x =>
            {
                x.Add<IEventFilter<Event>>().Instance(filter);
                x.Add<IEventHandler<Event>>().Instance(handler);
            });

            var mediator = new Mediator(container.GetService, new Configuration { EventPipelineEnabled = true });

            await Task.WhenAll(mediator.Publish(new Event()));

            filter.Executed.ShouldBeTrue();
            handler.Executed.ShouldBeTrue();
        }

        private class Event : IEvent { }

        private class EventFilter : IEventFilter<Event>
        {
            public bool Executed { get; set; }

            public Task Execute(EventFilterContext<Event> context,
                Func<Task> next)
            {
                Executed = true;
                return next();
            }
        }

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