﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    internal class CustomizedEventPipeline<TEvent> : EventPipeline<TEvent>
        where TEvent : IEvent
    {
        public IEventHandlerExecutionStrategy? HandlerExecutionStrategy { get; set; }
        public List<IEventPipelineBuilder> PipelineBuilders { get; set; } = null!;

        protected override async Task Execute(List<IEventFilter<TEvent>> filters, List<IEventHandler<TEvent>> handlers, EventContext<TEvent> eventContext)
        {
            var pipelineBuilderContext = new EventPipelineBuilderContext<TEvent>
            {
                Filters = filters,
                Handlers = handlers,
                HandlerExecutionStrategy = HandlerExecutionStrategy
            };

            foreach (var pipelineBuilder in PipelineBuilders)
            {
                await pipelineBuilder.Execute(pipelineBuilderContext, eventContext).WithGloballyConfiguredAwait();
            }

            eventContext.CancellationToken.ThrowIfCancellationRequested();

            var handlerExecutionStrategy = pipelineBuilderContext.HandlerExecutionStrategy ?? MediatorDefaults.EventHandlerExecutionStrategy;

            await Execute(filters, handlers, handlerExecutionStrategy, eventContext).WithGloballyConfiguredAwait();
        }
    }
}