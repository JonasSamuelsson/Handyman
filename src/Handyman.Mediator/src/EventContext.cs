using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Handyman.Mediator
{
    public class EventContext<TEvent>
    {
        public CancellationToken CancellationToken { get; set; }
        [DisallowNull, NotNull] public TEvent Event { get; set; } = default!;
        public IServiceProvider ServiceProvider { get; set; } = null!;
    }
}