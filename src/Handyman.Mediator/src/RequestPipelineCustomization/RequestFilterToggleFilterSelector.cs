using Handyman.Mediator.Internals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    internal class RequestFilterToggleFilterSelector : IRequestFilterSelector
    {
        private readonly RequestFilterToggleMetaData _toggleMetaData;

        public RequestFilterToggleFilterSelector(RequestFilterToggleMetaData toggleMetaData)
        {
            _toggleMetaData = toggleMetaData;
        }

        public async Task SelectFilters<TRequest, TResponse>(List<IRequestFilter<TRequest, TResponse>> filters, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>
        {
            var toggle = context.ServiceProvider.GetRequiredService<IRequestFilterToggle>();
            var enabled = await toggle.IsEnabled<TRequest, TResponse>(_toggleMetaData, context).ConfigureAwait();

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