using Handyman.Mediator;
using Handyman.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatorSamples.RequestFilters
{
    public class Program
    {
        public static async Task Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddMediator(options => options.ScanEntryAssembly())
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
            Console.WriteLine("handler");
            return Task.FromResult(request.Message);
        }
    }

    internal class EchoFilter : IRequestFilter<Echo, string>
    {
        public async Task<string> Execute(RequestPipelineContext<Echo> context, RequestFilterExecutionDelegate<string> next)
        {
            Console.WriteLine("echo filter");
            return await next();
        }
    }

    internal class GenericFilter<TRequest, TResponse> : IRequestFilter<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public Task<TResponse> Execute(RequestPipelineContext<TRequest> context, RequestFilterExecutionDelegate<TResponse> next)
        {
            Console.WriteLine("generic filter");
            return next();
        }
    }
}
