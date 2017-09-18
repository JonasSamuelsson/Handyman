using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IAsyncRequestHandler<TRequest> : IRequestHandler<TRequest, Task>
       where TRequest : IAsyncRequest
    {
    }

    public interface IAsyncRequestHandler<TRequest, TResponse> : IRequestHandler<TRequest, Task<TResponse>>
       where TRequest : IAsyncRequest<TResponse>
    {
    }
}