
using Handyman.Mediator;
using Handyman.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatorSamples.Requests
{
    public class Program
    {
        public static async Task Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddMediator(options => options.ScanAssemblyContaining<Program>())
                .BuildServiceProvider();

            var mediator = serviceProvider.GetRequiredService<IMediator>();

            var response = await mediator.Send(new Echo { Message = "Hello world!" });

            Console.WriteLine(response);
        }
    }

    internal class Echo : IRequest<string>
    {
        public string Message { get; set; }
    }

    internal class EchoHandler : IRequestHandler<Echo, string>
    {
        public Task<string> Handle(Echo request, CancellationToken cancellationToken)
        {
            return Task.FromResult(request.Message);
        }
    }
}
