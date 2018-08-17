using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IMediator
    {
        IEnumerable<Task> Publish<TEvent>(TEvent @event) where TEvent : IEvent;
        Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
    }
}