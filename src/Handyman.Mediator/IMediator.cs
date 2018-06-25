using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IMediator
    {
        IEnumerable<Task> Publish(IEvent @event);
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request);
    }
}