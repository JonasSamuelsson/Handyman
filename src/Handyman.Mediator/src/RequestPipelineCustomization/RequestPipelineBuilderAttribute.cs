using System;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class RequestPipelineBuilderAttribute : Attribute
    {
        public abstract void Configure<TRequest, TResponse>(IRequestPipelineBuilder<TRequest, TResponse> builder,
            ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>;
    }
}