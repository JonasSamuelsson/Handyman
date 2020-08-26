﻿using System;

namespace Handyman.Mediator.Pipeline
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class EventPipelineBuilderAttribute : Attribute
    {
        public abstract void Configure(IEventPipelineBuilder builder, IServiceProvider serviceProvider);
    }
}