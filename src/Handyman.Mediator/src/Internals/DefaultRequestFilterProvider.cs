using System.Collections.Generic;

namespace Handyman.Mediator.Internals
{
    internal class DefaultRequestFilterProvider : IRequestFilterProvider
    {
        internal static readonly IRequestFilterProvider Instance = new DefaultRequestFilterProvider();

        private DefaultRequestFilterProvider() { }

        public virtual IEnumerable<IRequestFilter<TRequest, TResponse>> GetFilters<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
        {
            return (IEnumerable<IRequestFilter<TRequest, TResponse>>)serviceProvider.Invoke(typeof(IEnumerable<IRequestFilter<TRequest, TResponse>>));
        }
    }
}