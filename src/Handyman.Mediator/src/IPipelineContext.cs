using System;
using System.Threading;

namespace Handyman.Mediator
{
    public interface IPipelineContext
    {
        CancellationToken CancellationToken { get; }
        object Message { get; }
        IServiceProvider ServiceProvider { get; }
    }
}