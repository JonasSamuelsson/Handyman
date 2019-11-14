using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public interface IEventHandlerExecutionStrategy<TEvent> where TEvent : IEvent
    {
        Task Execute(List<IEventHandler<TEvent>> handlers, IEventPipelineContext<TEvent> context);
    }
}