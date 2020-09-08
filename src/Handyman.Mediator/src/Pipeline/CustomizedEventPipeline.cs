using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    internal class CustomizedEventPipeline<TEvent> : EventPipeline<TEvent>
        where TEvent : IEvent
    {
        public List<IEventFilterSelector> FilterSelectors { get; set; }
        public List<IEventHandlerSelector> HandlerSelectors { get; set; }
        public IEventHandlerExecutionStrategy HandlerExecutionStrategy { get; set; }

        protected override async Task Execute(List<IEventFilter<TEvent>> filters, List<IEventHandler<TEvent>> handlers, EventContext<TEvent> eventContext)
        {
            foreach (var filterSelector in FilterSelectors)
            {
                eventContext.CancellationToken.ThrowIfCancellationRequested();
                await filterSelector.SelectFilters(filters, eventContext).WithGloballyConfiguredAwait();
            }

            foreach (var handlerSelector in HandlerSelectors)
            {
                eventContext.CancellationToken.ThrowIfCancellationRequested();
                await handlerSelector.SelectHandlers(handlers, eventContext).WithGloballyConfiguredAwait();
            }

            eventContext.CancellationToken.ThrowIfCancellationRequested();
            await Execute(filters, handlers, HandlerExecutionStrategy, eventContext).WithGloballyConfiguredAwait();
        }
    }
}