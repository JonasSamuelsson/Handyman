using System.Collections.Generic;
using System.Linq;

namespace Handyman.Mediator.Internals
{
    internal class NoRequestPipelineHandlerProvider : IRequestPipelineHandlerProvider
    {
        internal static readonly NoRequestPipelineHandlerProvider Instance = new NoRequestPipelineHandlerProvider();

        public IEnumerable<IRequestPipelineHandler<TRequest, TResponse>> GetHandlers<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
        {
            return Enumerable.Empty<IRequestPipelineHandler<TRequest, TResponse>>();
        }
    }
}