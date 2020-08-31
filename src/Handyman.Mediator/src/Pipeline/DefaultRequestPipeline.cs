using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    internal class DefaultRequestPipeline<TRequest, TResponse> : RequestPipeline<TRequest, TResponse>
       where TRequest : IRequest<TResponse>
    {
        internal static readonly RequestPipeline<TRequest, TResponse> Instance = new DefaultRequestPipeline<TRequest, TResponse>();

        private DefaultRequestPipeline() { }

        internal override Task<TResponse> Execute(RequestContext<TRequest> requestContext)
        {
            var filters = requestContext.ServiceProvider.GetServices<IRequestFilter<TRequest, TResponse>>().ToListOptimized();
            var handler = requestContext.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

            return Execute(filters, handler, DefaultRequestHandlerExecutionStrategy.Instance, requestContext);
        }
    }
}