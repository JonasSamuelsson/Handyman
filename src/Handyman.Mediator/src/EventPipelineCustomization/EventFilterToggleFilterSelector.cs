using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    internal class EventFilterToggleFilterSelector : IEventFilterSelector
    {
        private readonly EventFilterToggleInfo _toggleInfo;

        public EventFilterToggleFilterSelector(EventFilterToggleInfo toggleInfo)
        {
            _toggleInfo = toggleInfo;
        }

        public async Task SelectFilters<TEvent>(List<IEventFilter<TEvent>> filters, EventPipelineContext<TEvent> context) where TEvent : IEvent
        {
            var toggle = context.ServiceProvider.GetRequiredService<IEventFilterToggle>();
            var enabled = await toggle.IsEnabled(_toggleInfo, context).ConfigureAwait(false);

            if (!enabled)
            {
                filters.RemoveAll(x => x.GetType() == _toggleInfo.ToggleEnabledFilterType);
            }
            else if (_toggleInfo.ToggleDisabledFilterType != null)
            {
                filters.RemoveAll(x => x.GetType() == _toggleInfo.ToggleDisabledFilterType);
            }
        }
    }
}