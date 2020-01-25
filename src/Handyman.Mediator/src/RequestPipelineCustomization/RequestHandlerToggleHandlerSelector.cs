using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    internal class RequestHandlerToggleHandlerSelector : IRequestHandlerSelector
    {
        private readonly RequestHandlerToggleInfo _toggleInfo;

        public RequestHandlerToggleHandlerSelector(RequestHandlerToggleInfo toggleInfo)
        {
            _toggleInfo = toggleInfo;
        }

        public async Task SelectHandlers<TRequest, TResponse>(List<IRequestHandler<TRequest, TResponse>> handlers, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>
        {
            var toggle = context.ServiceProvider.GetRequiredService<IRequestHandlerToggle>();
            var enabled = await toggle.IsEnabled<TRequest, TResponse>(_toggleInfo, context).ConfigureAwait(false);

            if (!enabled)
            {
                handlers.RemoveAll(x => x.GetType() == _toggleInfo.ToggleEnabledHandlerType);
            }
            else if (_toggleInfo.ToggleDisabledHandlerType != null)
            {
                handlers.RemoveAll(x => x.GetType() == _toggleInfo.ToggleDisabledHandlerType);
            }
        }
    }
}