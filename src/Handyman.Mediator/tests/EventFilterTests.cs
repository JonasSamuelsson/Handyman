using Lamar;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class EventFilterTests
    {
        [Fact]
        public async Task ShouldExecuteFilters()
        {
            // using Lamar for dependency injection as it has support for constrained open generics

            var container = new Container(services => services.AddTransient<IEventFilter<IMarker>, Filter>());

            await new Mediator(container).Publish(new Event());

            Filter.Executed.ShouldBeTrue();
        }

        private class Event : IEvent, IMarker { }

        private interface IMarker { }

        private class Filter : IEventFilter<IMarker>
        {
            public static bool Executed;

            public Task Execute(IMarker @event, IEventFilterContext context, EventFilterExecutionDelegate next)
            {
                Executed = true;
                return Task.CompletedTask;
            }
        }
    }
}