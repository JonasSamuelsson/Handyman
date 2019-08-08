using System.Collections.Generic;

namespace Handyman.Mediator
{
    public interface IRequestFilterProvider
    {
        IEnumerable<IRequestFilter<TRequest, TResponse>> GetFilters<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>;
    }
}