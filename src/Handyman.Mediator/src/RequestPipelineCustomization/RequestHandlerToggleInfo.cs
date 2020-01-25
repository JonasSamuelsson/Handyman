using System;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class RequestHandlerToggleInfo
    {
        public Type ToggleDisabledHandlerType { get; internal set; }
        public Type ToggleEnabledHandlerType { get; internal set; }
        public string ToggleName { get; internal set; }
    }
}