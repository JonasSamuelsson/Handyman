using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IRequestPipelineHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<TRequest, CancellationToken, Task<TResponse>> next);
    }
}