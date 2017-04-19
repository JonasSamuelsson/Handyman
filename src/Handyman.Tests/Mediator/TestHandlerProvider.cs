using System;
using System.Collections.Generic;
using System.Linq;
using Handyman.Mediator;

namespace Handyman.Tests.Mediator
{
    internal class TestHandlerProvider : IHandlerProvider
    {
        private readonly Dictionary<Type, IEnumerable<Type>> _handlers;

        public TestHandlerProvider(Type handlerInterface, params Type[] handlerTypes)
        {
            _handlers = new Dictionary<Type, IEnumerable<Type>>
            {
                [handlerInterface] = handlerTypes
            };
        }

        public TestHandlerProvider(Dictionary<Type, IEnumerable<Type>> handlers)
        {
            _handlers = handlers;
        }

        public object GetHandler(Type handlerType)
        {
            return GetHandlers(handlerType).Single();
        }

        public IEnumerable<object> GetHandlers(Type handlerType)
        {
            return _handlers[handlerType].Select(Activator.CreateInstance);
        }
    }
}