using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IMediator
    {
        void Publish(IEvent @event);
        IEnumerable<Task> Publish(IAsyncEvent @event);

        void Send(IRequest request);
        TResponse Send<TResponse>(IRequest<TResponse> request);
    }
}