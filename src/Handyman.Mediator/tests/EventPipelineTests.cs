using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventPipelineTests
    {
        [Fact]
        public async Task ShouldExecuteEventFilters()
        {
            var filter = new EventFilter();
            var handler = new EventHandler();

            var services = new ServiceCollection();

            services.AddSingleton<IEventFilter<Event>>(filter);
            services.AddSingleton<IEventHandler<Event>>(handler);

            var mediator = new Mediator(services.BuildServiceProvider());

            await Task.WhenAll(mediator.Publish(new Event()));

            filter.Executed.ShouldBeTrue();
            handler.Executed.ShouldBeTrue();
        }

        private class Event : IEvent { }

        private class EventFilter : IEventFilter<Event>
        {
            public bool Executed { get; set; }

            public Task Execute(Event @event, IEventFilterContext context, EventFilterExecutionDelegate next)
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