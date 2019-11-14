using System;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class RequestHandlerToggleAttribute : RequestPipelineBuilderAttribute
    {
        private readonly Type _toggledHandlerType;

        public RequestHandlerToggleAttribute(Type toggledHandlerType)
        {
            _toggledHandlerType = toggledHandlerType;
        }

        public Type DefaultHandlerType { get; set; }

        public override void Configure<TRequest, TResponse>(IRequestPipelineBuilder<TRequest, TResponse> builder, ServiceProvider serviceProvider)
        {
            builder.AddHandlerSelector(new RequestHandlerToggleHandlerSelector<TRequest, TResponse>(_toggledHandlerType)
            {
                DefaultHandlerType = DefaultHandlerType
            });
        }
    }
}