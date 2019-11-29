using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class RequestHandlerToggleHandlerSelector<TRequest, TResponse> : IRequestHandlerSelector<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly Type _toggleEnabledHandlerType;

        public RequestHandlerToggleHandlerSelector(Type toggleEnabledHandlerType)
        {
            _toggleEnabledHandlerType = toggleEnabledHandlerType;
        }

        public Type ToggleDisabledHandlerType { get; set; }

        public async Task SelectHandlers(List<IRequestHandler<TRequest, TResponse>> handlers, RequestPipelineContext<TRequest> context)
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