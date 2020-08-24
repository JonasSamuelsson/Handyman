using System.Threading.Tasks;

namespace Handyman.Mediator.Pipelines
{
    internal delegate Task<TResponse> RequestHandlerDelegate<TRequest, TResponse>(RequestPipelineContext<TRequest> context);
}