using Handyman.Mediator.Pipeline;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IEventFilter<TEvent>
    {
        Task Execute(EventPipelineContext<TEvent> pipelineContext, EventFilterExecutionDelegate next);
    }
}