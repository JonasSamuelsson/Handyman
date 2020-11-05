using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Samples
{
    public class RequestFilterShortCircuitSample : ISample
    {
        public int Order => 125;
        public string Name => "Short circuit filter";

        public async Task Run(IMediator mediator)
        {
            Console.WriteLine($"Response from {await mediator.Send(new Request(), CancellationToken.None)}");
        }

        public class Request : IRequest<string> { }

        public class Handler : SyncRequestHandler<Request, string>
        {
            protected override string Handle(Request request, CancellationToken cancellationToken)
            {
                Console.WriteLine("Handler executing");
                return "handler";
            }
        }

        public class Filter : IRequestFilter<Request, string>
        {
            public Task<string> Execute(RequestContext<Request> requestContext, RequestFilterExecutionDelegate<string> next)
            {
                return Task.FromResult("filter");
            }
        }
    }
}