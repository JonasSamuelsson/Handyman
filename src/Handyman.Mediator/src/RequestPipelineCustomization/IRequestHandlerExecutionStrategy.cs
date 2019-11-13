using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public interface IRequestHandlerExecutionStrategy<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Execute(List<IRequestHandler<TRequest, TResponse>> handlers, IRequestPipelineContext<TRequest> context);
    }
}