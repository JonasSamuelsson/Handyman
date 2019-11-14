using System.Threading;

namespace Handyman.Mediator
{
    public interface IRequestFilterContext<TRequest>
    {
        CancellationToken CancellationToken { get; set; }
        TRequest Request { get; set; }
    }
}