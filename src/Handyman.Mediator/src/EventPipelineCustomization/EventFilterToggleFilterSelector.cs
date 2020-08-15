using Handyman.Mediator.Internals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    internal class EventFilterToggleFilterSelector : IEventFilterSelector
    {
        private readonly EventFilterToggleMetadata _toggleMetadata;

        public EventFilterToggleFilterSelector(EventFilterToggleMetadata toggleMetadata)
        {
            _toggleMetadata = toggleMetadata;
        }

        public async Task SelectFilters<TEvent>(List<IEventFilter<TEvent>> filters, EventPipelineContext<TEvent> context) where TEvent : IEvent
        {
            var toggle = context.ServiceProvider.GetRequiredService<IEventFilterToggle>();
            var enabled = await toggle.IsEnabled(_toggleMetadata, context).ConfigureAwait();

            if (!enabled)
            {
                filters.RemoveAll(x => x.GetType() == _toggleMetadata.ToggleEnabledFilterType);
            }
            else if (_toggleMetadata.ToggleDisabledFilterType != null)
            {
                filters.RemoveAll(x => x.GetType() == _toggleMetadata.ToggleDisabledFilterType);
            }
        }
    }
}