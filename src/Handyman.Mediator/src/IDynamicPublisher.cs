using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IDynamicPublisher
    {
        Task Publish(object @event, CancellationToken cancellationToken);
    }
}