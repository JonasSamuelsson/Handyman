using System;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.WhenAnyRequestHandler
{
    [AttributeUsage(AttributeTargets.Class)]
    public class WhenAnyRequestHandlerAttribute : RequestPipelineBuilderAttribute
    {
        public override Task Execute<TRequest, TResponse>(RequestPipelineBuilderContext<TRequest, TResponse> pipelineBuilderContext, RequestContext<TRequest> requestContext)
        {
            var handlers = pipelineBuilderContext.Handlers;

            if (handlers.Count > 1)
            {
                var handler = new WhenAnyRequestHandler<TRequest, TResponse>(handlers.ToList(), requestContext.ServiceProvider);
                handlers.Clear();
                handlers.Add(handler);
            }

            return Task.CompletedTask;
        }
    }
}