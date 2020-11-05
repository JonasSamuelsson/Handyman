using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Samples
{
    public class OrderedFiltersSample : ISample
    {
        public int Order => 120;
        public string Name => "Ordered filters";

        public async Task Run(IMediator mediator)
        {
            await mediator.Send(new Request(), CancellationToken.None);
        }

        public class Request : IRequest { }

        public class Handler : SyncRequestHandler<Request>
        {
            protected override void Handle(Request request, CancellationToken cancellationToken)
            {
                Console.WriteLine("Handler");
            }
        }

        public class FirstFilter<TRequest, TResponse> : IRequestFilter<TRequest, TResponse>, IOrderedFilter
        {
            public int Order => -100;

            public async Task<TResponse> Execute(RequestContext<TRequest> requestContext, RequestFilterExecutionDelegate<TResponse> next)
            {
                Console.WriteLine("First filter preprocessing");
                var response = await next();
                Console.WriteLine("First filter postprocessing");
                return response;
            }
        }

        public class SecondFilter<TRequest, TResponse> : IRequestFilter<TRequest, TResponse>
        {
            public async Task<TResponse> Execute(RequestContext<TRequest> requestContext, RequestFilterExecutionDelegate<TResponse> next)
            {
                Console.WriteLine("Second filter preprocessing");
                var response = await next();
                Console.WriteLine("Second filter postprocessing");
                return response;
            }
        }

        public class ThirdFilter<TRequest, TResponse> : IRequestFilter<TRequest, TResponse>, IOrderedFilter
        {
            public int Order => 100;

            public async Task<TResponse> Execute(RequestContext<TRequest> requestContext, RequestFilterExecutionDelegate<TResponse> next)
            {
                Console.WriteLine("Third filter preprocessing");
                var response = await next();
                Console.WriteLine("Third filter postprocessing");
                return response;
            }
        }
    }
}