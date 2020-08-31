using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    public interface IRequestHandlerExecutionStrategy
    {
        Task<TResponse> Execute<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler,
            RequestContext<TRequest> requestContext) where TRequest : IRequest<TResponse>;
    }
}