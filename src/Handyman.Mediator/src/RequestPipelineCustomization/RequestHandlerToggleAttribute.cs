using System;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequestHandlerToggleAttribute : RequestPipelineBuilderAttribute
    {
        private readonly Type _toggleEnabledHandlerType;

        public RequestHandlerToggleAttribute(Type toggleEnabledHandlerType)
        {
            _toggleEnabledHandlerType = toggleEnabledHandlerType ?? throw new ArgumentNullException(nameof(toggleEnabledHandlerType));
        }

        public string Name { get; set; }
        public string[] Tags { get; set; }
        public Type ToggleDisabledHandlerType { get; set; }

        public override void Configure(IRequestPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            var toggleInfo = new RequestHandlerToggleMetadata
            {
                Name = Name,
                Tags = Tags,
                ToggleDisabledHandlerType = ToggleDisabledHandlerType,
                ToggleEnabledHandlerType = _toggleEnabledHandlerType
            };

            builder.AddHandlerSelector(new RequestHandlerToggleHandlerSelector(toggleInfo));
        }
    }
}