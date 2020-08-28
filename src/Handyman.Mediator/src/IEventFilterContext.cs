using System;
using System.Threading;

namespace Handyman.Mediator
{
    public interface IEventFilterContext
    {
        CancellationToken CancellationToken { get; set; }
        IServiceProvider ServiceProvider { get; set; }
    }
}