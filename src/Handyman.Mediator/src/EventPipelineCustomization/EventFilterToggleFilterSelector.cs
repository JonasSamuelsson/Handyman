using Handyman.Mediator.Internals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    internal class EventFilterToggleFilterSelector : IEventFilterSelector
    {
        private readonly EventFilterToggleMetaData _toggleMetaData;

        public EventFilterToggleFilterSelector(EventFilterToggleMetaData toggleMetaData)
        {
            _toggleMetaData = toggleMetaData;
        }

        public async Task SelectFilters<TEvent>(List<IEventFilter<TEvent>> filters, EventPipelineContext<TEvent> context) where TEvent : IEvent
        {
            var toggle = context.ServiceProvider.GetRequiredService<IEventFilterToggle>();
            var enabled = await toggle.IsEnabled(_toggleMetaData, context).ConfigureAwait();

            if (!enabled)
            {
                filters.RemoveAll(x => x.GetType() == _toggleMetaData.ToggleEnabledFilterType);
            }
            else if (_toggleMetaData.ToggleDisabledFilterType != null)
            {
                filters.RemoveAll(x => x.GetType() == _toggleMetaData.ToggleDisabledFilterType);
            }
        }
    }
}