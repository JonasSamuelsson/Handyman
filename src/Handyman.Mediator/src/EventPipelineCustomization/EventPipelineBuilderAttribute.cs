﻿using System;

namespace Handyman.Mediator.EventPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class EventPipelineBuilderAttribute : Attribute
    {
        public abstract void Configure<TEvent>(IEventPipelineBuilder<TEvent> builder, ServiceProvider serviceProvider)
            where TEvent : IEvent;
    }
}