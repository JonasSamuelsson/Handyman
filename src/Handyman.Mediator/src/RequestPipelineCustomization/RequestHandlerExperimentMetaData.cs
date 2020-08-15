using Handyman.Mediator.PipelineCustomization;
using System;
using System.Collections.Generic;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class RequestHandlerExperimentMetadata : IToggleMetadata
    {
        public Type BaselineHandlerType { get; internal set; }
        public string Name { get; internal set; }
        public IEnumerable<string> Tags { get; internal set; }
    }
}