using System.Threading.Tasks;
using Handyman.Mediator.Pipelines;

namespace Handyman.Mediator
{
    public interface IRequestFilter<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Execute(RequestPipelineContext<TRequest> context, RequestFilterExecutionDelegate<TResponse> next);
    }
}