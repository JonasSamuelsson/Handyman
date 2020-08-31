using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    internal delegate Task<TResponse> RequestHandlerDelegate<TRequest, TResponse>(RequestContext<TRequest> requestContext);
}