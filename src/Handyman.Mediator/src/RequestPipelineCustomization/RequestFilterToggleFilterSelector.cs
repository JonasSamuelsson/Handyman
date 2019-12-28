using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    internal class RequestFilterToggleFilterSelector : IRequestFilterSelector
    {
        private readonly Type _toggleEnabledFilterType;

        public RequestFilterToggleFilterSelector(Type toggleEnabledFilterType)
        {
            _toggleEnabledFilterType = toggleEnabledFilterType;
        }

        public Type ToggleDisabledFilterType { get; set; }

        public async Task SelectFilters<TRequest, TResponse>(List<IRequestFilter<TRequest, TResponse>> filters, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>
        {
            var toggle = context.ServiceProvider.GetRequiredService<IRequestFilterToggle>();
            var enabled = await toggle.IsEnabled<TRequest, TResponse>(_toggleEnabledFilterType, context).ConfigureAwait(false);

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