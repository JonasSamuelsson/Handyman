using Handyman.Mediator.EventPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Threading.Tasks;
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
            var toggledHandler = new ToggledEventHandler();
            var fallbackHandler = new FallbackEventHandler();

            var services = new ServiceCollection();

            services.AddScoped<IMediator>(x => new Mediator(x));
            services.AddSingleton<IEventHandlerToggle<Event>>(toggle);
            services.AddSingleton<IEventHandler<Event>>(toggledHandler);
            services.AddSingleton<IEventHandler<Event>>(fallbackHandler);

            await services.BuildServiceProvider().GetService<IMediator>().Publish(new Event());

            toggledHandler.Executed.ShouldBe(toggleEnabled);
            fallbackHandler.Executed.ShouldBe(!toggleEnabled);
        }

        [EventHandlerToggle(typeof(ToggledEventHandler), FallbackHandlerType = typeof(FallbackEventHandler))]
        private class Event : IEvent { }

        private class ToggledEventHandler : EventHandler<Event>
        {
            public bool Executed { get; set; }

            protected override void Handle(Event @event)
            {
                Executed = true;
            }
        }

        private class FallbackEventHandler : EventHandler<Event>
        {
            public bool Executed { get; set; }

            protected override void Handle(Event @event)
            {
                Executed = true;
            }
        }

        private class EventHandlerToggle : IEventHandlerToggle<Event>
        {
            public bool Enabled { get; set; }

            public Task<bool> IsEnabled(EventPipelineContext<Event> context)
            {
                return Task.FromResult(Enabled);
            }
        }
    }
}