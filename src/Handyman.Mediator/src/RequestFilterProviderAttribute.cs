using System;
using System.Collections.Generic;

namespace Handyman.Mediator
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public abstract class RequestFilterProviderAttribute : Attribute, IRequestFilterProvider
    {
        public abstract IEnumerable<IRequestFilter<TRequest, TResponse>> GetFilters<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>;
    }
}