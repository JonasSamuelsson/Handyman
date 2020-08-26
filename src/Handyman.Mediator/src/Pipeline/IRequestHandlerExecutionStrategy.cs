using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    public interface IRequestHandlerExecutionStrategy
    {
        Task<TResponse> Execute<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler,
            RequestPipelineContext<TRequest> context) where TRequest : IRequest<TResponse>;
    }
}