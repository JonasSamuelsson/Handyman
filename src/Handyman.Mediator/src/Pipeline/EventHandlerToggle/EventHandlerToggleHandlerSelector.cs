using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.EventHandlerToggle
{
    internal class EventHandlerToggleHandlerSelector : IEventHandlerSelector
    {
        private readonly EventHandlerToggleMetadata _toggleMetadata;

        public EventHandlerToggleHandlerSelector(EventHandlerToggleMetadata toggleMetadata)
        {
            _toggleMetadata = toggleMetadata;
        }

        public async Task SelectHandlers<TEvent>(List<IEventHandler<TEvent>> handlers, EventContext<TEvent> eventContext)
            where TEvent : IEvent
        {
            var toggle = eventContext.ServiceProvider.GetRequiredService<IEventHandlerToggle>();
            var enabled = await toggle.IsEnabled(_toggleMetadata, eventContext).WithGloballyConfiguredAwait();

            if (!enabled)
            {
                handlers.RemoveAll(x => _toggleMetadata.ToggleEnabledHandlerTypes.Contains(x.GetType()));
            }
            else if (_toggleMetadata.ToggleDisabledHandlerTypes != null)
            {
                handlers.RemoveAll(x => _toggleMetadata.ToggleDisabledHandlerTypes.Contains(x.GetType()));
            }
        }
    }
}