using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Dispatch
{
    public interface IDispatcher
    {
        Task<TResponse> Process<TResponse>(IRequest<TResponse> request);
        IEnumerable<Task> Publish(IMessage message);
    }
}