using Handyman.Mediator.Internals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    internal class RequestFilterToggleFilterSelector : IRequestFilterSelector
    {
        private readonly RequestFilterToggleMetadata _toggleMetadata;

        public RequestFilterToggleFilterSelector(RequestFilterToggleMetadata toggleMetadata)
        {
            _toggleMetadata = toggleMetadata;
        }

        public async Task SelectFilters<TRequest, TResponse>(List<IRequestFilter<TRequest, TResponse>> filters, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>
        {
            var toggle = context.ServiceProvider.GetRequiredService<IRequestFilterToggle>();
            var enabled = await toggle.IsEnabled<TRequest, TResponse>(_toggleMetadata, context).ConfigureAwait();

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