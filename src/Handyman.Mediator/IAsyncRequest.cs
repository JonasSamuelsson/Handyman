using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IAsyncRequest : IRequest<Task>
    {
    }

    public interface IAsyncRequest<TResponse> : IRequest<Task<TResponse>>
    {
    }
}