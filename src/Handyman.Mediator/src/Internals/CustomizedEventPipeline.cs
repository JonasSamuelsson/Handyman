using Handyman.Mediator.EventPipelineCustomization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Internals
{
    internal class CustomizedEventPipeline<TEvent> : EventPipeline<TEvent>
        where TEvent : IEvent
    {
        public List<IEventFilterSelector<TEvent>> FilterSelectors { get; set; }
        public List<IEventHandlerSelector> HandlerSelectors { get; set; }
        public IEventHandlerExecutionStrategy HandlerExecutionStrategy { get; set; }

        protected override async Task Execute(List<IEventFilter<TEvent>> filters, List<IEventHandler<TEvent>> handlers, EventPipelineContext<TEvent> context)
        {
            foreach (var filterSelector in FilterSelectors)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                await filterSelector.SelectFilters(filters, context).ConfigureAwait(false);
            }

            foreach (var handlerSelector in HandlerSelectors)
            {
                context.CancellationToken.ThrowIfCancellationRequested();
                await handlerSelector.SelectHandlers(handlers, context).ConfigureAwait(false);
            }

            context.CancellationToken.ThrowIfCancellationRequested();
            await Execute(filters, handlers, HandlerExecutionStrategy, context).ConfigureAwait(false);
        }
    }
}