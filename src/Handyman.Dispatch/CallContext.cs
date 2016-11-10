using System;

namespace Handyman.Dispatch
{
   internal class CallContext
   {
      public Func<object, object> AdapterFactory { get; set; }
      public Type HandlerInterface { get; set; }
   }
}