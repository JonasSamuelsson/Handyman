using Handyman.Mediator.Pipeline;
using Handyman.Mediator.Pipeline.EventFilterToggle;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests.Pipeline.EventFilterToggle
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
            services.AddSingleton<IEventFilterToggle>(toggle);
            services.AddSingleton<IEventFilter<Event>>(toggledFilter);
            services.AddSingleton<IEventFilter<Event>>(fallbackFilter);

            await services.BuildServiceProvider().GetService<IMediator>().Publish(new Event());

            toggle.ToggleMetadata.Name.ShouldBe("test");
            toggle.ToggleMetadata.Tags.ShouldBe(new[] { "foo" });
            toggle.ToggleMetadata.ToggleDisabledFilterTypes.ShouldBe(new[] { typeof(ToggleDisabledEventFilter) });
            toggle.ToggleMetadata.ToggleEnabledFilterTypes.ShouldBe(new[] { typeof(ToggleEnabledEventFilter) });

            toggledFilter.Executed.ShouldBe(toggleEnabled);
            fallbackFilter.Executed.ShouldBe(!toggleEnabled);
        }

        [EventFilterToggle(typeof(ToggleEnabledEventFilter), ToggleDisabledFilterTypes = new[] { typeof(ToggleDisabledEventFilter) }, Name = "test", Tags = new[] { "foo" })]
        private class Event : IEvent { }

        private class ToggleEnabledEventFilter : IEventFilter<Event>
        {
            public bool Executed { get; set; }

            public Task Execute(EventContext<Event> eventContext, EventFilterExecutionDelegate next)
            {
                Executed = true;
                return next();
            }
        }

        private class ToggleDisabledEventFilter : IEventFilter<Event>
        {
            public bool Executed { get; set; }

            public Task Execute(EventContext<Event> eventContext, EventFilterExecutionDelegate next)
            {
                Executed = true;
                return next();
            }
        }

        private class EventFilterToggle : IEventFilterToggle
        {
            public bool Enabled { get; set; }
            public EventFilterToggleMetadata ToggleMetadata { get; set; }

            public Task<bool> IsEnabled<TEvent>(EventFilterToggleMetadata toggleMetadata, EventContext<TEvent> eventContext)
                where TEvent : IEvent
            {
                ToggleMetadata = toggleMetadata;
                return Task.FromResult(Enabled);
            }
        }
    }
}