using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Handyman.Mediator
{
    public class RequestContext<TRequest>
    {
        public CancellationToken CancellationToken { get; set; }
        [DisallowNull, NotNull] public TRequest Request { get; set; } = default!;
        public IServiceProvider ServiceProvider { get; set; } = null!;
    }
}