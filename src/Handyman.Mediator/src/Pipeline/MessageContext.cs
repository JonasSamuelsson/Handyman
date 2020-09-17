using System;
using System.Threading;

namespace Handyman.Mediator.Pipeline
{
    public class MessageContext
    {
        public CancellationToken CancellationToken { get; set; }
        public object Message { get; set; } = null!;
        public IServiceProvider ServiceProvider { get; set; } = null!;
    }
}