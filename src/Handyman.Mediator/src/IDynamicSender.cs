using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public interface IDynamicSender
    {
        Task<object?> Send(object request, CancellationToken cancellationToken);
    }
}