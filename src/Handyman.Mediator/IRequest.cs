namespace Handyman.Mediator
{
    public interface IRequest : IRequest<Void> { }

    public interface IRequest<TResponse> { }
}