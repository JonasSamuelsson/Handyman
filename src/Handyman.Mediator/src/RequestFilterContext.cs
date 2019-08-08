using System.Threading;

namespace Handyman.Mediator
{
    public class RequestFilterContext<TRequest>
    {
        public CancellationToken CancellationToken { get; set; }
        public TRequest Request { get; set; }
    }
}