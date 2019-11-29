﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    internal class EventFilterToggleFilterSelector<TEvent> : IEventFilterSelector<TEvent>
        where TEvent : IEvent
    {
        private readonly Type _toggleEnabledFilterType;

        public EventFilterToggleFilterSelector(Type toggleEnabledFilterType)
        {
            _toggleEnabledFilterType = toggleEnabledFilterType;
        }

        public Type ToggleDisabledFilterType { get; set; }

        public async Task SelectFilters(List<IEventFilter<TEvent>> filters, EventPipelineContext<TEvent> context)
        {
            var toggle = context.ServiceProvider.GetRequiredService<IEventFilterToggle<TEvent>>();
            var enabled = await toggle.IsEnabled(_toggleEnabledFilterType, context).ConfigureAwait(false);

            if (!enabled)
            {
                filters.RemoveAll(x => x.GetType() == _toggleEnabledFilterType);
            }
            else if (ToggleDisabledFilterType != null)
            {
                filters.RemoveAll(x => x.GetType() == ToggleDisabledFilterType);
            }
        }
    }
}