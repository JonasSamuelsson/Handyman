using System;
using System.Collections.Generic;

namespace Handyman.Mediator
{
   public interface IHandlerProvider
   {
      object GetHandler(Type handlerInterface);
      IEnumerable<object> GetHandlers(Type handlerInterface);
   }
}