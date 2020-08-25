using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    internal class DefaultRequestPipeline<TRequest, TResponse> : RequestPipeline<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        internal static readonly RequestPipeline<TRequest, TResponse> Instance = new DefaultRequestPipeline<TRequest, TResponse>();

        private DefaultRequestPipeline() { }

        internal override Task<TResponse> Execute(RequestPipelineContext<TRequest> context)
        {
            var filters = context.ServiceProvider.GetServices<IRequestFilter<TRequest, TResponse>>().ToListOptimized();
            var handler = context.ServiceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();

            return Execute(filters, ctx => handler.Handle(ctx.Request, ctx.CancellationToken), context);
        }
    }
}