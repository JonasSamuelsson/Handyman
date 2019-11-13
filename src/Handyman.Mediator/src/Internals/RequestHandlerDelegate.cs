using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
{
    internal delegate Task<TResponse> RequestHandlerDelegate<TRequest, TResponse>(RequestPipelineContext<TRequest> context);
}