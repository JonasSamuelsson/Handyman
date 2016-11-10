using System;
using System.Collections.Generic;

namespace Handyman.Dispatch
{
   public interface IHandlerProvider
   {
      object GetHandler(Type handlerInterface);
      IEnumerable<object> GetHandlers(Type handlerInterface);
   }
}