using Handyman.Mediator.Internals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    internal class EventHandlerToggleHandlerSelector : IEventHandlerSelector
    {
        private readonly EventHandlerToggleMetadata _toggleMetadata;

        public EventHandlerToggleHandlerSelector(EventHandlerToggleMetadata toggleMetadata)
        {
            _toggleMetadata = toggleMetadata;
        }

        public async Task SelectHandlers<TEvent>(List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context) where TEvent : IEvent
        {
            var toggle = context.ServiceProvider.GetRequiredService<IEventHandlerToggle>();
            var enabled = await toggle.IsEnabled(_toggleMetadata, context).ConfigureAwait();

            if (!enabled)
            {
                handlers.RemoveAll(x => x.GetType() == _toggleMetadata.ToggleEnabledHandlerType);
            }
            else if (_toggleMetadata.ToggleDisabledHandlerType != null)
            {
                handlers.RemoveAll(x => x.GetType() == _toggleMetadata.ToggleDisabledHandlerType);
            }
        }
    }
}