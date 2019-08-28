using System.Linq;
using System.Threading;
using Handyman.Mediator;

namespace ExperimentRequestHandler
{
    public class NewAddHandler : RequestHandler<Add, int>
    {
        protected override int Handle(Add request, CancellationToken cancellationToken)
        {
            return request.Numbers.Contains(0) ? 0 : request.Numbers.Sum();
        }
    }
}