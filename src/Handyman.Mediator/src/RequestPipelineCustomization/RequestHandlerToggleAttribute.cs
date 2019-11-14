using System;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RequestHandlerToggleAttribute : RequestPipelineBuilderAttribute
    {
        private readonly Type _toggledHandlerType;

        public RequestHandlerToggleAttribute(Type toggledHandlerType)
        {
            _toggledHandlerType = toggledHandlerType ?? throw new ArgumentNullException(nameof(toggledHandlerType));
        }

        public Type FallbackHandlerType { get; set; }

        public override void Configure<TRequest, TResponse>(IRequestPipelineBuilder<TRequest, TResponse> builder, ServiceProvider serviceProvider)
        {
            builder.AddHandlerSelector(new RequestHandlerToggleHandlerSelector<TRequest, TResponse>(_toggledHandlerType)
            {
                FallbackHandlerType = FallbackHandlerType
            });
        }
    }
}