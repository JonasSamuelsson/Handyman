namespace Handyman.Mediator
{
    public interface IEventHandler<TEvent>
       where TEvent : IEvent
    {
        void Handle(TEvent @event);
    }
}