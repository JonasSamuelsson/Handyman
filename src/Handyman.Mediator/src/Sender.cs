using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public class Sender<TRequest> : ISender<TRequest>
        where TRequest : IRequest
    {
        private readonly IMediator _mediator;

        public Sender(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Send(TRequest request, CancellationToken cancellationToken)
        {
            return _mediator.Send(request, cancellationToken);
        }
    }

    public class Sender<TRequest, TResponse> : ISender<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IMediator _mediator;

        public Sender(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<TResponse> Send(TRequest request, CancellationToken cancellationToken)
        {
            return _mediator.Send(request, cancellationToken);
        }
    }
}