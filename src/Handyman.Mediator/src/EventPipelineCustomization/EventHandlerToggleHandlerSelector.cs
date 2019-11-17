using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    internal class EventHandlerToggleHandlerSelector<TEvent> : IEventHandlerSelector<TEvent>
        where TEvent : IEvent
    {
        private readonly Type _toggledHandlerType;

        public EventHandlerToggleHandlerSelector(Type toggledHandlerType)
        {
            _toggledHandlerType = toggledHandlerType;
        }

        public Type FallbackHandlerType { get; set; }

        public async Task SelectHandlers(List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context)
        {
            var toggle = context.ServiceProvider.GetRequiredService<IEventHandlerToggle<TEvent>>();
            var enabled = await toggle.IsEnabled(context).ConfigureAwait(false);

            if (!enabled)
            {
                handlers.RemoveAll(x => x.GetType() == _toggledHandlerType);
            }
            else if (FallbackHandlerType != null)
            {
                handlers.RemoveAll(x => x.GetType() == FallbackHandlerType);
            }
        }
    }
}