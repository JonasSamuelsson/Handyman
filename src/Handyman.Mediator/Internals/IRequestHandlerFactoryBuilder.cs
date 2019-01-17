using System;

namespace Handyman.Mediator.Internals
{
    internal interface IRequestHandlerFactoryBuilder
    {
        Func<ServiceProvider, object> BuildFactory(Type requestType, Type responseType);
    }
}