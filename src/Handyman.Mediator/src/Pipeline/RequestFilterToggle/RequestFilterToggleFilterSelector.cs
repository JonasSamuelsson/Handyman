using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.RequestFilterToggle
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
                filters.RemoveAll(x => _toggleMetadata.ToggleEnabledFilterTypes.Contains(x.GetType()));
            }
            else if (_toggleMetadata.ToggleDisabledFilterTypes != null)
            {
                filters.RemoveAll(x => _toggleMetadata.ToggleDisabledFilterTypes.Contains(x.GetType()));
            }
        }
    }
}