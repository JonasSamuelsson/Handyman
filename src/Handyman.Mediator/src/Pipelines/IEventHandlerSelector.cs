using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipelines
{
    public interface IEventHandlerSelector
    {
        Task SelectHandlers<TEvent>(List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context) where TEvent : IEvent;
    }
}