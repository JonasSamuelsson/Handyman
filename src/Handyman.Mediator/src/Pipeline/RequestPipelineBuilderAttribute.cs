using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class RequestPipelineBuilderAttribute : Attribute, IRequestPipelineBuilder, IOrderedPipelineBuilder
    {
        public int ExecutionOrder { get; set; } = Defaults.Order.Default;

        public abstract Task Execute<TRequest, TResponse>(RequestPipelineBuilderContext<TRequest, TResponse> pipelineBuilderContext, RequestContext<TRequest> requestContext)
            where TRequest : IRequest<TResponse>;
    }
}