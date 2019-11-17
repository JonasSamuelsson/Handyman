using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public interface IEventHandlerSelector<TEvent> where TEvent : IEvent
    {
        Task SelectHandlers(List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context);
    }
}