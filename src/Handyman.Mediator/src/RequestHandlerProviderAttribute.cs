using System;

namespace Handyman.Mediator
{
    [AttributeUsage(AttributeTargets.Class)]
    public abstract class RequestHandlerProviderAttribute : Attribute, IRequestHandlerProvider
    {
        public abstract IRequestHandler<TRequest, TResponse> GetHandler<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>;
    }
}