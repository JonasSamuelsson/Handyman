using System;

namespace Handyman.Mediator.Internals
{
    internal interface IEventHandlerFactoryBuilder
    {
        Func<Func<Type, object>, object> BuildFactory(Type eventType);
    }
}