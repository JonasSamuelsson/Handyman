using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Samples
{
    public class RequestFilterChangeResponseSample : ISample
    {
        public int Order => 130;
        public string Name => "Request filter modify response";

        public async Task Run(IMediator mediator)
        {
            Console.WriteLine(await mediator.Send(new Request(), CancellationToken.None));
        }

        public class Request : IRequest<string> { }

        public class Handler : SyncRequestHandler<Request, string>
        {
            protected override string Handle(Request request, CancellationToken cancellationToken)
            {
                return "Handler";
            }
        }

        public class Filter : IRequestFilter<Request, string>
        {
            public async Task<string> Execute(RequestContext<Request> requestContext, RequestFilterExecutionDelegate<string> next)
            {
                return $"{await next()} =)";
            }
        }
    }
}