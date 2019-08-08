namespace Handyman.Mediator.Internals
{
    internal class DefaultRequestHandlerProvider : IRequestHandlerProvider
    {
        internal static readonly DefaultRequestHandlerProvider Instance = new DefaultRequestHandlerProvider();

        private DefaultRequestHandlerProvider() { }

        public IRequestHandler<TRequest, TResponse> GetHandler<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
        {
            var type = typeof(IRequestHandler<TRequest, TResponse>);
            return (IRequestHandler<TRequest, TResponse>)serviceProvider.Invoke(type);
        }
    }
}