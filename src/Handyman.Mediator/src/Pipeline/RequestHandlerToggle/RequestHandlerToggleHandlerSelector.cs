using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.RequestHandlerToggle
{
    internal class RequestHandlerToggleHandlerSelector : IRequestHandlerSelector
    {
        private readonly RequestHandlerToggleMetadata _toggleMetadata;

        public RequestHandlerToggleHandlerSelector(RequestHandlerToggleMetadata toggleMetadata)
        {
            _toggleMetadata = toggleMetadata;
        }

        public async Task SelectHandlers<TRequest, TResponse>(List<IRequestHandler<TRequest, TResponse>> handlers, RequestContext<TRequest> requestContext)
            where TRequest : IRequest<TResponse>
        {
            var toggle = requestContext.ServiceProvider.GetRequiredService<IRequestHandlerToggle>();
            var enabled = await toggle.IsEnabled<TRequest, TResponse>(_toggleMetadata, requestContext).ConfigureAwait();

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