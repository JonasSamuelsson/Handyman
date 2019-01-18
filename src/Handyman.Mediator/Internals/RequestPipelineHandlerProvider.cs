using System.Collections.Generic;

namespace Handyman.Mediator.Internals
{
    internal class RequestPipelineHandlerProvider : IRequestPipelineHandlerProvider
    {
        internal static readonly RequestPipelineHandlerProvider Instance = new RequestPipelineHandlerProvider();

        public IEnumerable<IRequestPipelineHandler<TRequest, TResponse>> GetHandlers<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
        {
            var type = typeof(IEnumerable<IRequestPipelineHandler<TRequest, TResponse>>);
            return (IEnumerable<IRequestPipelineHandler<TRequest, TResponse>>)serviceProvider.Invoke(type);
        }
    }
}