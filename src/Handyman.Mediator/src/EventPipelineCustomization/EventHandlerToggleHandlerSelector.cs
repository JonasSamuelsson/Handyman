using Handyman.Mediator.Internals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    internal class EventHandlerToggleHandlerSelector : IEventHandlerSelector
    {
        private readonly EventHandlerToggleMetaData _toggleMetaData;

        public EventHandlerToggleHandlerSelector(EventHandlerToggleMetaData toggleMetaData)
        {
            _toggleMetaData = toggleMetaData;
        }

        public async Task SelectHandlers<TEvent>(List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context) where TEvent : IEvent
        {
            var toggle = context.ServiceProvider.GetRequiredService<IEventHandlerToggle>();
            var enabled = await toggle.IsEnabled(_toggleMetaData, context).ConfigureAwait();

            if (!enabled)
            {
                handlers.RemoveAll(x => x.GetType() == _toggleMetaData.ToggleEnabledHandlerType);
            }
            else if (_toggleMetaData.ToggleDisabledHandlerType != null)
            {
                handlers.RemoveAll(x => x.GetType() == _toggleMetaData.ToggleDisabledHandlerType);
            }
        }
    }
}