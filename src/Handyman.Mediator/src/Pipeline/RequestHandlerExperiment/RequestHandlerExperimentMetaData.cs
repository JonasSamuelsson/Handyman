using System;
using System.Collections.Generic;

namespace Handyman.Mediator.Pipeline.RequestHandlerExperiment
{
    public class RequestHandlerExperimentMetadata : IToggleMetadata
    {
        public Type BaselineHandlerType { get; internal set; } = null!;
        public Type[] ExperimentalHandlerTypes { get; internal set; } = null!;
        public string? Name { get; internal set; }
        public IEnumerable<string>? Tags { get; internal set; }
        public ToggleFailureMode? FailureMode { get; set; }
    }
}