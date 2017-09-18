using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IMediator
    {
        void Send(IRequest request);
        TResponse Send<TResponse>(IRequest<TResponse> request);
        IEnumerable<Task> Publish(IMessage message);
    }
}