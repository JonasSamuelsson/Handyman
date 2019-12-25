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

        public Type ToggleDisabledHandlerType { get; set; }

        public override void Configure(IRequestPipelineBuilder builder, IServiceProvider serviceProvider)
        {
            builder.AddHandlerSelector(new RequestHandlerToggleHandlerSelector(_toggleEnabledHandlerType)
            {
                ToggleDisabledHandlerType = ToggleDisabledHandlerType
            });
        }
    }
}