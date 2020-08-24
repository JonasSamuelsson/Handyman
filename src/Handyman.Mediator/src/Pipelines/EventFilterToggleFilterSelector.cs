using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipelines
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
                filters.RemoveAll(x => _toggleMetadata.ToggleEnabledFilterTypes.Contains(x.GetType()));
            }
            else if (_toggleMetadata.ToggleDisabledFilterTypes.Any())
            {
                filters.RemoveAll(x => _toggleMetadata.ToggleDisabledFilterTypes.Contains(x.GetType()));
            }
        }
    }
}