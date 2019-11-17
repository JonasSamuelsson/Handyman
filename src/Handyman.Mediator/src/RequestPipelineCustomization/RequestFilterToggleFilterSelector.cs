using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    internal class RequestFilterToggleFilterSelector<TRequest, TResponse> : IRequestFilterSelector<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly Type _toggledFilterType;

        public RequestFilterToggleFilterSelector(Type toggledFilterType)
        {
            _toggledFilterType = toggledFilterType;
        }

        public Type FallbackFilterType { get; set; }

        public async Task SelectFilters(List<IRequestFilter<TRequest, TResponse>> filters, RequestPipelineContext<TRequest> context)
        {
            var toggle = context.ServiceProvider.GetRequiredService<IRequestFilterToggle<TRequest, TResponse>>();
            var enabled = await toggle.IsEnabled(context).ConfigureAwait(false);

            if (!enabled)
            {
                filters.RemoveAll(x => x.GetType() == _toggledFilterType);
            }
            else if (FallbackFilterType != null)
            {
                filters.RemoveAll(x => x.GetType() == FallbackFilterType);
            }
        }
    }
}