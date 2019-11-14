using System;
using System.Collections.Generic;
using System.Linq;
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

        public Type DefaultHandlerType { get; set; }

        public async Task SelectHandlers(List<IRequestHandler<TRequest, TResponse>> handlers, IRequestPipelineContext<TRequest> context)
        {
            var toggle = context.ServiceProvider.GetRequiredService<IRequestHandlerToggle<TRequest, TResponse>>();
            var defaultHandler = handlers.Single(x => x.GetType() == _toggledHandlerType);
            var toggledHandler = handlers.Single(x => x.GetType() != _toggledHandlerType);
            var enabled = await toggle.IsEnabled(context.Request).ConfigureAwait(false);

            if (!enabled)
            {
                handlers.RemoveAll(x => x.GetType() == _toggledHandlerType);
            }
            else if (DefaultHandlerType != null)
            {
                handlers.RemoveAll(x => x.GetType() == DefaultHandlerType);
            }
        }
    }
}