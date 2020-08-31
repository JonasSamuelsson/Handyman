using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.WhenAnyRequestHandler
{
    internal class WhenAnyRequestHandlerSelector : IRequestHandlerSelector
    {
        public Task SelectHandlers<TRequest, TResponse>(List<IRequestHandler<TRequest, TResponse>> handlers, RequestContext<TRequest> requestContext)
            where TRequest : IRequest<TResponse>
        {
            if (handlers.Count > 1)
            {
                var handler = new WhenAnyRequestHandler<TRequest, TResponse>(handlers.ToList(), requestContext.ServiceProvider);
                handlers.Clear();
                handlers.Add(handler);
            }

            return Task.CompletedTask;
        }
    }
}