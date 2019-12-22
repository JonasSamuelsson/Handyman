using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    internal class EventHandlerToggleHandlerSelector : IEventHandlerSelector
    {
        private readonly Type _toggleEnabledHandlerType;

        public EventHandlerToggleHandlerSelector(Type toggleEnabledHandlerType)
        {
            _toggleEnabledHandlerType = toggleEnabledHandlerType;
        }

        public Type ToggleDisabledHandlerType { get; set; }

        public async Task SelectHandlers<TEvent>(List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context) where TEvent : IEvent
        {
            var toggle = context.ServiceProvider.GetRequiredService<IEventHandlerToggle<TEvent>>();
            var enabled = await toggle.IsEnabled(_toggleEnabledHandlerType, context).ConfigureAwait(false);

            if (!enabled)
            {
                handlers.RemoveAll(x => x.GetType() == _toggleEnabledHandlerType);
            }
            else if (ToggleDisabledHandlerType != null)
            {
                handlers.RemoveAll(x => x.GetType() == ToggleDisabledHandlerType);
            }
        }
    }
}