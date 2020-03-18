using Handyman.Mediator;
using Handyman.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatorSamples.ManipulatingRequestFilters
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
            return Task.FromResult(request.Message);
        }
    }

    internal class Filter : IRequestFilter<Echo, string>
    {
        public async Task<string> Execute(RequestPipelineContext<Echo> context, RequestFilterExecutionDelegate<string> next)
        {
            context.Request.Message = $"pre processing: {context.Request.Message}";
            return $"post processing: {await next()}";
        }
    }
}
