using Handyman.Mediator;
using Handyman.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatorSamples.TerminatingRequestFilters
{
    public class Program
    {
        public static async Task Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddMediator(options => options.ScanAssemblyContaining<Program>())
                .BuildServiceProvider();

            var mediator = serviceProvider.GetRequiredService<IMediator>();

            Console.WriteLine(await mediator.Send(new Request { Terminate = false }));
            Console.WriteLine(await mediator.Send(new Request { Terminate = true }));
        }
    }

    internal class Request : IRequest<string>
    {
        public bool Terminate { get; set; }
    }

    internal class Handler : IRequestHandler<Request, string>
    {
        public Task<string> Handle(Request request, CancellationToken cancellationToken)
        {
            return Task.FromResult("handled");
        }
    }

    internal class TerminatingFilter : IRequestFilter<Request, string>
    {
        public Task<string> Execute(RequestPipelineContext<Request> context, RequestFilterExecutionDelegate<string> next)
        {
            return context.Request.Terminate
                ? Task.FromResult("terminated")
                : next();
        }
    }
}
