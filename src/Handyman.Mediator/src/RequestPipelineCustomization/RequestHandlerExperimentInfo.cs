using System;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class RequestHandlerExperimentInfo
    {
        public Type BaselineHandlerType { get; internal set; }
        public string ExperimentName { get; internal set; }
    }
}