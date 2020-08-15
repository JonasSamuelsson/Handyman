using System;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class RequestHandlerExperimentMetaData
    {
        public Type BaselineHandlerType { get; internal set; }
        public string ExperimentName { get; internal set; }
        public string[] Tags { get; set; }
    }
}