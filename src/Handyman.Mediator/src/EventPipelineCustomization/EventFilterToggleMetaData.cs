﻿using Handyman.Mediator.PipelineCustomization;
using System;
using System.Collections.Generic;

namespace Handyman.Mediator.EventPipelineCustomization
{
    public class EventFilterToggleMetadata : IToggleMetadata
    {
        public string Name { get; internal set; }
        public IEnumerable<string> Tags { get; internal set; }
        public IEnumerable<Type> ToggleDisabledFilterTypes { get; internal set; }
        public IEnumerable<Type> ToggleEnabledFilterTypes { get; internal set; }
    }
}