using Handyman.Mediator;
using Handyman.Mediator.DependencyInjection;
using Handyman.Mediator.RequestPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatorSamples.WhenAnyRequestHandler
{
    public class Program
    {
        public static async Task Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddMediator(options => options.ScanAssemblyContaining<Program>())
                .BuildServiceProvider();

            var mediator = serviceProvider.GetRequiredService<IMediator>();

            for (var i = 0; i < 4; i++)
            {
                Console.WriteLine(await mediator.Send(new Request()));
            }
        }
    }

    [WhenAnyRequestHandler]
    internal class Request : IRequest<string> { }

    internal class Handler1 : IRequestHandler<Request, string>
    {
        private static int _counter;

        public async Task<string> Handle(Request request, CancellationToken cancellationToken)
        {
            var milliseconds = _counter++ % 2 == 0 ? 0 : 100;
            await Task.Delay(milliseconds, cancellationToken);
            return "handler 1";
        }
    }

    internal class Handler2 : IRequestHandler<Request, string>
    {
        public async Task<string> Handle(Request request, CancellationToken cancellationToken)
        {
            await Task.Delay(50, cancellationToken);
            return "handler 2";
        }
    }
}