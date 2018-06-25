using System;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IRequestPipelineHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Execute(TRequest request, Func<TRequest, Task<TResponse>> next);
    }
}