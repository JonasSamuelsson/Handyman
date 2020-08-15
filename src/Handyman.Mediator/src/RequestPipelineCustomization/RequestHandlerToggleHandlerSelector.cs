using Handyman.Mediator.Internals;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    internal class RequestHandlerToggleHandlerSelector : IRequestHandlerSelector
    {
        private readonly RequestHandlerToggleMetaData _toggleMetaData;

        public RequestHandlerToggleHandlerSelector(RequestHandlerToggleMetaData toggleMetaData)
        {
            _toggleMetaData = toggleMetaData;
        }

        public async Task SelectHandlers<TRequest, TResponse>(List<IRequestHandler<TRequest, TResponse>> handlers, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>
        {
            var toggle = context.ServiceProvider.GetRequiredService<IRequestHandlerToggle>();
            var enabled = await toggle.IsEnabled<TRequest, TResponse>(_toggleMetaData, context).ConfigureAwait();

            if (!enabled)
            {
                handlers.RemoveAll(x => x.GetType() == _toggleMetaData.ToggleEnabledHandlerType);
            }
            else if (_toggleMetaData.ToggleDisabledHandlerType != null)
            {
                handlers.RemoveAll(x => x.GetType() == _toggleMetaData.ToggleDisabledHandlerType);
            }
        }
    }
}