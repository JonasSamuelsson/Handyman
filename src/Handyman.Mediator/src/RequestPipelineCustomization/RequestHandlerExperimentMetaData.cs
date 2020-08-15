using System;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class RequestHandlerExperimentMetaData
    {
        public Type BaselineHandlerType { get; internal set; }
        public string Name { get; internal set; }
        public string[] Tags { get; set; }
    }
}