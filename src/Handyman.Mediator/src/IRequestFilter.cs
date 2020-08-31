using System.Threading.Tasks;
using Handyman.Mediator.Pipeline;

namespace Handyman.Mediator
{
    public interface IRequestFilter<TRequest, TResponse>
    {
        Task<TResponse> Execute(RequestPipelineContext<TRequest> context, RequestFilterExecutionDelegate<TResponse> next);
    }
}