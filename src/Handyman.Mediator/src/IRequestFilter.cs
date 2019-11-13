using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IRequestFilter<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Execute(IRequestFilterContext<TRequest> context, RequestFilterExecutionDelegate<TResponse> next);
    }
}