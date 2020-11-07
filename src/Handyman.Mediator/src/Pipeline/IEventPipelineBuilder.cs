using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    public interface IEventPipelineBuilder
    {
        Task Execute<TEvent>(EventPipelineBuilderContext<TEvent> pipelineBuilderContext, EventContext<TEvent> eventContext)
            where TEvent : IEvent;
    }
}