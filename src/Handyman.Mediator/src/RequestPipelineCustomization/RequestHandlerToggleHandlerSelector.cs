using Handyman.Mediator.Internals;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    internal class RequestHandlerToggleHandlerSelector : IRequestHandlerSelector
    {
        private readonly RequestHandlerToggleMetadata _toggleMetadata;

        public RequestHandlerToggleHandlerSelector(RequestHandlerToggleMetadata toggleMetadata)
        {
            _toggleMetadata = toggleMetadata;
        }

        public async Task SelectHandlers<TRequest, TResponse>(List<IRequestHandler<TRequest, TResponse>> handlers, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>
        {
            var toggle = context.ServiceProvider.GetRequiredService<IRequestHandlerToggle>();
            var enabled = await toggle.IsEnabled<TRequest, TResponse>(_toggleMetadata, context).ConfigureAwait();

            if (!enabled)
            {
                handlers.RemoveAll(x => _toggleMetadata.ToggleEnabledHandlerTypes.Contains(x.GetType()));
            }
            else if (_toggleMetadata.ToggleDisabledHandlerTypes != null)
            {
                handlers.RemoveAll(x => _toggleMetadata.ToggleDisabledHandlerTypes.Contains(x.GetType()));
            }
        }
    }
}