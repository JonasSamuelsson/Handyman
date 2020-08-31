using Handyman.Mediator.Pipeline;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IRequestFilter<TRequest, TResponse>
    {
        Task<TResponse> Execute(RequestContext<TRequest> requestContext, RequestFilterExecutionDelegate<TResponse> next);
    }
}