using Handyman.Mediator.Internals;
using System;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequestHandlerExperimentAttribute : RequestPipelineBuilderAttribute
    {
        private readonly Type _baselineHandlerType;

        public RequestHandlerExperimentAttribute(Type baselineHandlerType)
        {
            _baselineHandlerType = baselineHandlerType;
        }

        public string ExperimentName { get; set; }

        public override void Configure(IRequestPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            var experimentInfo = new RequestHandlerExperimentInfo
            {
                BaselineHandlerType = _baselineHandlerType,
                ExperimentName = ExperimentName
            };

            builder.UseHandlerExecutionStrategy(new RequestHandlerExperimentExecutionStrategy(experimentInfo));
        }
    }
}