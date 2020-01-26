using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    internal class EventHandlerToggleHandlerSelector : IEventHandlerSelector
    {
        private readonly EventHandlerToggleInfo _toggleInfo;

        public EventHandlerToggleHandlerSelector(EventHandlerToggleInfo toggleInfo)
        {
            _toggleInfo = toggleInfo;
        }

        public async Task SelectHandlers<TEvent>(List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context) where TEvent : IEvent
        {
            var toggle = context.ServiceProvider.GetRequiredService<IEventHandlerToggle>();
            var enabled = await toggle.IsEnabled(_toggleInfo, context).ConfigureAwait(false);

            if (!enabled)
            {
                handlers.RemoveAll(x => x.GetType() == _toggleInfo.ToggleEnabledHandlerType);
            }
            else if (_toggleInfo.ToggleDisabledHandlerType != null)
            {
                handlers.RemoveAll(x => x.GetType() == _toggleInfo.ToggleDisabledHandlerType);
            }
        }
    }
}