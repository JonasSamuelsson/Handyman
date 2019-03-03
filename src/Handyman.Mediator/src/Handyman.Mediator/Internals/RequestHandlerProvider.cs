namespace Handyman.Mediator.Internals
{
    internal class RequestHandlerProvider : IRequestHandlerProvider
    {
        internal static readonly RequestHandlerProvider Instance = new RequestHandlerProvider();

        public IRequestHandler<TRequest, TResponse> GetHandler<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>
        {
            var type = typeof(IRequestHandler<TRequest, TResponse>);
            return (IRequestHandler<TRequest, TResponse>)serviceProvider.Invoke(type);
        }
    }
}