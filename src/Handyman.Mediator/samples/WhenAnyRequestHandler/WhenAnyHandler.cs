using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Samples.WhenAnyRequestHandler
{
    public class WhenAnyHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IRequestHandler<TRequest, TResponse>> _handlers;

        public WhenAnyHandler(IEnumerable<IRequestHandler<TRequest, TResponse>> handlers)
        {
            _handlers = handlers;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
        {
            // note that this isn't production ready as it doesn't do things like cancel the other handlers once the first one has completed etc
            return await await Task.WhenAny(_handlers.Select(x => x.Handle(request, cancellationToken)));
        }
    }
}