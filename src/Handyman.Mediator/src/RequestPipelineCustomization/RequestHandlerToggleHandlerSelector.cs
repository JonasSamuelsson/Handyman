using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class RequestHandlerToggleHandlerSelector : IRequestHandlerSelector
    {
        private readonly Type _toggleEnabledHandlerType;

        public RequestHandlerToggleHandlerSelector(Type toggleEnabledHandlerType)
        {
            _toggleEnabledHandlerType = toggleEnabledHandlerType;
        }

        public Type ToggleDisabledHandlerType { get; set; }

        public async Task SelectHandlers<TRequest, TResponse>(List<IRequestHandler<TRequest, TResponse>> handlers, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>
        {
            var toggle = context.ServiceProvider.GetRequiredService<IRequestHandlerToggle<TRequest, TResponse>>();
            var enabled = await toggle.IsEnabled(_toggleEnabledHandlerType, context).ConfigureAwait(false);

            if (!enabled)
            {
                handlers.RemoveAll(x => x.GetType() == _toggleEnabledHandlerType);
            }
            else if (ToggleDisabledHandlerType != null)
            {
                handlers.RemoveAll(x => x.GetType() == ToggleDisabledHandlerType);
            }
        }
    }
}