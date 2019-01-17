using System;

namespace Handyman.Mediator.Internals
{
    internal interface IEventHandlerFactoryBuilder
    {
        Func<ServiceProvider, object> BuildFactory(Type eventType);
    }
}