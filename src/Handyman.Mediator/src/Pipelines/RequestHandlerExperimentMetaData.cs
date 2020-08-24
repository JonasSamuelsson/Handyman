using System;
using System.Collections.Generic;

namespace Handyman.Mediator.Pipelines
{
    public class RequestHandlerExperimentMetadata : IToggleMetadata
    {
        public Type BaselineHandlerType { get; internal set; }
        public string Name { get; internal set; }
        public IEnumerable<string> Tags { get; internal set; }
    }
}