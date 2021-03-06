﻿using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class EventPipelineBuilderAttribute : Attribute, IEventPipelineBuilder, IOrderedPipelineBuilder
    {
        public int Order { get; set; } = MediatorDefaults.Order.Default;

        public abstract Task Execute<TEvent>(EventPipelineBuilderContext<TEvent> pipelineBuilderContext,
            EventContext<TEvent> eventContext) where TEvent : IEvent;
    }
}