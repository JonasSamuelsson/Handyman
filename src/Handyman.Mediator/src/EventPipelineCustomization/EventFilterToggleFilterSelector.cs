using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    internal class EventFilterToggleFilterSelector<TEvent> : IEventFilterSelector<TEvent>
        where TEvent : IEvent
    {
        private readonly Type _toggledFilterType;

        public EventFilterToggleFilterSelector(Type toggledFilterType)
        {
            _toggledFilterType = toggledFilterType;
        }

        public Type FallbackFilterType { get; set; }

        public async Task SelectFilters(List<IEventFilter<TEvent>> filters, EventPipelineContext<TEvent> context)
        {
            var toggle = context.ServiceProvider.GetRequiredService<IEventFilterToggle<TEvent>>();
            var enabled = await toggle.IsEnabled(context).ConfigureAwait(false);

            if (!enabled)
            {
                filters.RemoveAll(x => x.GetType() == _toggledFilterType);
            }
            else if (FallbackFilterType != null)
            {
                filters.RemoveAll(x => x.GetType() == FallbackFilterType);
            }
        }
    }
}