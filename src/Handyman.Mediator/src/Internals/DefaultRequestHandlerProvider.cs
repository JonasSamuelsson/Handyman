using System.Linq;
using System.Reflection;

namespace Handyman.Mediator.Internals
{
    internal class DefaultRequestHandlerProvider : IRequestHandlerProvider
    {
        internal static readonly DefaultRequestHandlerProvider Instance = new DefaultRequestHandlerProvider();

        private DefaultRequestHandlerProvider() { }

        public IRequestHandler<TRequest, TResponse> GetHandler<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
        {
            var attribute = typeof(TRequest).GetCustomAttributes<RequestHandlerProviderAttribute>(true).SingleOrDefault();

            if (attribute != null)
                return attribute.GetHandler<TRequest, TResponse>(serviceProvider);

            return (IRequestHandler<TRequest, TResponse>)serviceProvider.Invoke(typeof(IRequestHandler<TRequest, TResponse>));
        }
    }
}