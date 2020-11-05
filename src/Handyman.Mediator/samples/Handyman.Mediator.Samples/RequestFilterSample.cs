using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Samples
{
    public class RequestFilterSample : ISample
    {
        public int Order => 110;
        public string Name => "Filters";

        public async Task Run(IMediator mediator)
        {
            await mediator.Send(new Request(), CancellationToken.None);
        }

        public class Request : IRequest { }

        public class Handler : SyncRequestHandler<Request>
        {
            protected override void Handle(Request request, CancellationToken cancellationToken)
            {
                Console.WriteLine("Handled by handler");
            }
        }

        public class Filter<TRequest, TResponse> : IRequestFilter<TRequest, TResponse>
        {
            public async Task<TResponse> Execute(RequestContext<TRequest> requestContext, RequestFilterExecutionDelegate<TResponse> next)
            {
                Console.WriteLine("Filter preprocessing");
                var response = await next();
                Console.WriteLine("Filter postprocessing");
                return response;
            }
        }
    }
}