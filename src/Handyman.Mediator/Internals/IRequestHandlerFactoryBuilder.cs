using System;

namespace Handyman.Mediator.Internals
{
    internal interface IRequestHandlerFactoryBuilder
    {
        Func<Func<Type, object>, object> BuildFactory(Type requestType, Type responseType);
    }
}