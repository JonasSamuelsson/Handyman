using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestHandlerSelector<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task SelectHandlers(List<IRequestHandler<TRequest, TResponse>> handlers, IRequestPipelineContext<TRequest> context);
    }
}