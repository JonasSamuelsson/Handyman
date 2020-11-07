using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    internal class DefaultRequestHandlerExecutionStrategy : IRequestHandlerExecutionStrategy
    {
        public Task<TResponse> Execute<TRequest, TResponse>(IRequestHandler<TRequest, TResponse> handler,
            RequestContext<TRequest> requestContext)
            where TRequest : IRequest<TResponse>
        {
            return handler.Handle(requestContext.Request, requestContext.CancellationToken);
        }
    }
}