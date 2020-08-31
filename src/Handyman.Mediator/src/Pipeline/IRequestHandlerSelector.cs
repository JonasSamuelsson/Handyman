using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    public interface IRequestHandlerSelector
    {
        Task SelectHandlers<TRequest, TResponse>(List<IRequestHandler<TRequest, TResponse>> handlers, RequestContext<TRequest> requestContext)
            where TRequest : IRequest<TResponse>;
    }
}