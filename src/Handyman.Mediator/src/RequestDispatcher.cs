using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator
{
    public class RequestDispatcher<TRequest> : IRequestDispatcher<TRequest>
        where TRequest : IRequest
    {
        private readonly IMediator _mediator;

        public RequestDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Send(TRequest request, CancellationToken cancellationToken)
        {
            return _mediator.Send(request, cancellationToken);
        }
    }

    public class RequestDispatcher<TRequest, TResponse> : IRequestDispatcher<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IMediator _mediator;

        public RequestDispatcher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<TResponse> Send(TRequest request, CancellationToken cancellationToken)
        {
            return _mediator.Send(request, cancellationToken);
        }
    }
}