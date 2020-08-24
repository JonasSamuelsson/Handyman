using System;

namespace Handyman.Mediator.Pipelines
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequestHandlerExperimentAttribute : RequestPipelineBuilderAttribute
    {
        private readonly Type _baselineHandlerType;

        public RequestHandlerExperimentAttribute(Type baselineHandlerType)
        {
            _baselineHandlerType = baselineHandlerType ?? throw new ArgumentNullException(nameof(baselineHandlerType));
        }

        public string Name { get; set; }
        public string[] Tags { get; set; }

        public override void Configure(IRequestPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            var experimentInfo = new RequestHandlerExperimentMetadata
            {
                BaselineHandlerType = _baselineHandlerType,
                Name = Name,
                Tags = Tags
            };

            builder.UseHandlerExecutionStrategy(new RequestHandlerExperimentExecutionStrategy(experimentInfo));
        }
    }
}