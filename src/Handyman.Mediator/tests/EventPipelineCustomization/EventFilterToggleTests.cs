using System;
using System.Threading.Tasks;
using Handyman.Mediator.EventPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
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
            var toggledFilter = new ToggleEnabledEventFilter();
            var fallbackFilter = new ToggleDisabledEventFilter();

            var services = new ServiceCollection();

            services.AddScoped<IMediator>(x => new Mediator(x));
            services.AddSingleton<IEventFilterToggle<Event>>(toggle);
            services.AddSingleton<IEventFilter<Event>>(toggledFilter);
            services.AddSingleton<IEventFilter<Event>>(fallbackFilter);

            await services.BuildServiceProvider().GetService<IMediator>().Publish(new Event());

            toggle.EventFilterType.ShouldBe(typeof(ToggleEnabledEventFilter));
            toggledFilter.Executed.ShouldBe(toggleEnabled);
            fallbackFilter.Executed.ShouldBe(!toggleEnabled);
        }

        [EventFilterToggle(typeof(ToggleEnabledEventFilter), ToggleDisabledFilterType = typeof(ToggleDisabledEventFilter))]
        private class Event : IEvent { }

        private class ToggleEnabledEventFilter : IEventFilter<Event>
        {
            public bool Executed { get; set; }

            public Task Execute(EventPipelineContext<Event> context, EventFilterExecutionDelegate next)
            {
                Executed = true;
                return next();
            }
        }

        private class ToggleDisabledEventFilter : IEventFilter<Event>
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
            public Type EventFilterType { get; set; }

            public Task<bool> IsEnabled(Type eventFilterType, EventPipelineContext<Event> context)
            {
                EventFilterType = eventFilterType;
                return Task.FromResult(Enabled);
            }
        }
    }
}