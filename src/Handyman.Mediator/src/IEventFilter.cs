using Handyman.Mediator.Pipeline;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IEventFilter<TEvent>
    {
        Task Execute(EventContext<TEvent> eventContext, EventFilterExecutionDelegate next);
    }
}