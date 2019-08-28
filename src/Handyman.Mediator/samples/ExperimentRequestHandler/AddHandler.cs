using System.Linq;
using System.Threading;
using Handyman.Mediator;

namespace ExperimentRequestHandler
{
    public class AddHandler : RequestHandler<Add, int>
    {
        protected override int Handle(Add request, CancellationToken cancellationToken)
        {
            return request.Numbers.Sum();
        }
    }
}