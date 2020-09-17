using System;

namespace Handyman.Mediator.Pipeline.RequestHandlerExperiment
{
    [AttributeUsage(AttributeTargets.Class)]
    public class RequestHandlerExperimentAttribute : RequestPipelineBuilderAttribute
    {
        private readonly Type _baselineHandlerType;

        public RequestHandlerExperimentAttribute(Type baselineHandlerType)
        {
            _baselineHandlerType = baselineHandlerType ?? throw new ArgumentNullException(nameof(baselineHandlerType));
        }

        public Type[]? ExperimentalHandlerTypes { get; set; }
        public string? Name { get; set; }
        public string[]? Tags { get; set; }

        public override void Configure(IRequestPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            var metadata = new RequestHandlerExperimentMetadata
            {
                BaselineHandlerType = _baselineHandlerType,
                ExperimentalHandlerTypes = ExperimentalHandlerTypes ?? Type.EmptyTypes,
                Name = Name,
                Tags = Tags
            };

            builder.AddHandlerSelector(new RequestHandlerExperimentHandlerSelector(metadata, serviceProvider));
        }
    }
}