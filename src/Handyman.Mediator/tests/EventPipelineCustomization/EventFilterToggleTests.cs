using Handyman.Mediator.EventPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests.EventPipelineCustomization
{
    public class EventFilterToggleTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ShouldToggleEventFilter(bool toggleEnabled)
        {
            var toggle = new EventFilterToggle { Enabled = toggleEnabled };
            var toggledFilter = new ToggledEventFilter();
            var fallbackFilter = new FallbackEventFilter();

            var services = new ServiceCollection();

            services.AddScoped<IMediator>(x => new Mediator(x));
            services.AddSingleton<IEventFilterToggle<Event>>(toggle);
            services.AddSingleton<IEventFilter<Event>>(toggledFilter);
            services.AddSingleton<IEventFilter<Event>>(fallbackFilter);

            await services.BuildServiceProvider().GetService<IMediator>().Publish(new Event());

            toggledFilter.Executed.ShouldBe(toggleEnabled);
            fallbackFilter.Executed.ShouldBe(!toggleEnabled);
        }

        [EventFilterToggle(typeof(ToggledEventFilter), FallbackFilterType = typeof(FallbackEventFilter))]
        private class Event : IEvent { }

        private class ToggledEventFilter : IEventFilter<Event>
        {
            public bool Executed { get; set; }

            public Task Execute(EventPipelineContext<Event> context, EventFilterExecutionDelegate next)
            {
                Executed = true;
                return next();
            }
        }

        private class FallbackEventFilter : IEventFilter<Event>
        {
            public bool Executed { get; set; }

            public Task Execute(EventPipelineContext<Event> context, EventFilterExecutionDelegate next)
            {
                Executed = true;
                return next();
            }
        }

        private class EventFilterToggle : IEventFilterToggle<Event>
        {
            public bool Enabled { get; set; }

            public Task<bool> IsEnabled(EventPipelineContext<Event> context)
            {
                return Task.FromResult(Enabled);
            }
        }
    }
}