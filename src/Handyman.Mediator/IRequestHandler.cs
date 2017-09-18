namespace Handyman.Mediator
{
    public interface IRequestHandler<TRequest>
        where TRequest : IRequest
    {
        void Handle(TRequest request);
    }

    public interface IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        TResponse Handle(TRequest request);
    }
}