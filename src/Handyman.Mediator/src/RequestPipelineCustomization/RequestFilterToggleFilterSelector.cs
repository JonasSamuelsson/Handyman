using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    internal class RequestFilterToggleFilterSelector<TRequest, TResponse> : IRequestFilterSelector<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly Type _toggleEnabledFilterType;

        public RequestFilterToggleFilterSelector(Type toggleEnabledFilterType)
        {
            _toggleEnabledFilterType = toggleEnabledFilterType;
        }

        public Type ToggleDisabledFilterType { get; set; }

        public async Task SelectFilters(List<IRequestFilter<TRequest, TResponse>> filters, RequestPipelineContext<TRequest> context)
        {
            var toggle = context.ServiceProvider.GetRequiredService<IRequestFilterToggle<TRequest, TResponse>>();
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