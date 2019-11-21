using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class RequestHandlerToggleHandlerSelector<TRequest, TResponse> : IRequestHandlerSelector<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly Type _toggledHandlerType;

        public RequestHandlerToggleHandlerSelector(Type toggledHandlerType)
        {
            _toggledHandlerType = toggledHandlerType;
        }

        public Type FallbackHandlerType { get; set; }

        public async Task SelectHandlers(List<IRequestHandler<TRequest, TResponse>> handlers, RequestPipelineContext<TRequest> context)
        {
            var toggle = context.ServiceProvider.GetRequiredService<IRequestHandlerToggle<TRequest, TResponse>>();
            var enabled = await toggle.IsEnabled(_toggledHandlerType, context).ConfigureAwait(false);

            if (!enabled)
            {
                handlers.RemoveAll(x => x.GetType() == _toggledHandlerType);
            }
            else if (FallbackHandlerType != null)
            {
                handlers.RemoveAll(x => x.GetType() == FallbackHandlerType);
            }
        }
    }
}