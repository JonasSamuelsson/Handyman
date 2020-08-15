using Handyman.Mediator.Internals;
using System.Collections.Generic;
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
                handlers.RemoveAll(x => x.GetType() == _toggleMetadata.ToggleEnabledHandlerType);
            }
            else if (_toggleMetadata.ToggleDisabledHandlerType != null)
            {
                handlers.RemoveAll(x => x.GetType() == _toggleMetadata.ToggleDisabledHandlerType);
            }
        }
    }
}