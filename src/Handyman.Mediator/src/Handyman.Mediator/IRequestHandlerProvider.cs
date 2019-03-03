namespace Handyman.Mediator
{
    public interface IRequestHandlerProvider
    {
        IRequestHandler<TRequest, TResponse> GetHandler<TRequest, TResponse>(ServiceProvider serviceProvider) where TRequest : IRequest<TResponse>;
    }
}