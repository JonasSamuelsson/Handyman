using System;
using System.Threading.Tasks;
using Handyman.Mediator.EventPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests.EventPipelineCustomization
{
    public class EventHandlerToggleTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task ShouldToggleEventHandler(bool toggleEnabled)
        {
            var toggle = new EventHandlerToggle { Enabled = toggleEnabled };
            var toggledHandler = new ToggleEnabledEventHandler();
            var fallbackHandler = new ToggleDisabledEventHandler();

            var services = new ServiceCollection();

            services.AddScoped<IMediator>(x => new Mediator(x));
            services.AddSingleton<IEventHandlerToggle>(toggle);
            services.AddSingleton<IEventHandler<Event>>(toggledHandler);
            services.AddSingleton<IEventHandler<Event>>(fallbackHandler);

            await services.BuildServiceProvider().GetService<IMediator>().Publish(new Event());

            toggle.ToggleInfo.ToggleDisabledHandlerType.ShouldBe(typeof(ToggleDisabledEventHandler));
            toggle.ToggleInfo.ToggleEnabledHandlerType.ShouldBe(typeof(ToggleEnabledEventHandler));
            toggle.ToggleInfo.ToggleName.ShouldBe("test");
            toggledHandler.Executed.ShouldBe(toggleEnabled);
            fallbackHandler.Executed.ShouldBe(!toggleEnabled);
        }

        [EventHandlerToggle(typeof(ToggleEnabledEventHandler), ToggleDisabledHandlerType = typeof(ToggleDisabledEventHandler), ToggleName = "test")]
        private class Event : IEvent { }

        private class ToggleEnabledEventHandler : EventHandler<Event>
        {
            public bool Executed { get; set; }

            protected override void Handle(Event @event)
            {
                Executed = true;
            }
        }

        private class ToggleDisabledEventHandler : EventHandler<Event>
        {
            public bool Executed { get; set; }

            protected override void Handle(Event @event)
            {
                Executed = true;
            }
        }

        private class EventHandlerToggle : IEventHandlerToggle
        {
            public bool Enabled { get; set; }
            public EventHandlerToggleInfo ToggleInfo { get; set; }

            public Task<bool> IsEnabled<TEvent>(EventHandlerToggleInfo toggleInfo, EventPipelineContext<TEvent> context) where TEvent : IEvent
            {
                ToggleInfo = toggleInfo;
                return Task.FromResult(Enabled);
            }
        }
    }
}