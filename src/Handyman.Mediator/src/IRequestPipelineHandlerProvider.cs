using System.Collections.Generic;

namespace Handyman.Mediator
{
    public interface IRequestPipelineHandlerProvider
    {
        IEnumerable<IRequestPipelineHandler<TRequest, TResponse>> GetHandlers<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>;
    }
}