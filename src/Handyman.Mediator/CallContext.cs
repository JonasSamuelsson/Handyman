using System;

namespace Handyman.Mediator
{
   internal class CallContext
   {
      public Func<object, object> AdapterFactory { get; set; }
      public Type HandlerInterface { get; set; }
   }
}