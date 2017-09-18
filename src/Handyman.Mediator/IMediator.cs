using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IMediator
    {
        void Publish(IMessage message);
        IEnumerable<Task> Publish(IAsyncMessage message);

        void Send(IRequest request);
        TResponse Send<TResponse>(IRequest<TResponse> request);
    }
}