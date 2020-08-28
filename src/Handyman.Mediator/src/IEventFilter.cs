using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IEventFilter<in TEvent>
    {
        Task Execute(TEvent @event, IEventFilterContext context, EventFilterExecutionDelegate next);
    }
}