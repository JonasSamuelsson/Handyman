using System;
using System.Collections.Generic;

namespace Handyman.Mediator
{
    public interface IServiceProvider : System.IServiceProvider
    {
        IEnumerable<object> GetServices(Type serviceType);
    }
}