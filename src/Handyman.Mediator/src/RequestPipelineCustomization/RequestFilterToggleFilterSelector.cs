using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    internal class RequestFilterToggleFilterSelector : IRequestFilterSelector
    {
        private readonly RequestFilterToggleInfo _toggleInfo;

        public RequestFilterToggleFilterSelector(RequestFilterToggleInfo toggleInfo)
        {
            _toggleInfo = toggleInfo;
        }

        public async Task SelectFilters<TRequest, TResponse>(List<IRequestFilter<TRequest, TResponse>> filters, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>
        {
            var toggle = context.ServiceProvider.GetRequiredService<IRequestFilterToggle>();
            var enabled = await toggle.IsEnabled<TRequest, TResponse>(_toggleInfo, context).ConfigureAwait(false);

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