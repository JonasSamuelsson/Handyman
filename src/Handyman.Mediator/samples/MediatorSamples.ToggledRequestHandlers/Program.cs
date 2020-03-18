using Handyman.Mediator;
using Handyman.Mediator.DependencyInjection;
using Handyman.Mediator.RequestPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;
using Console = System.Console;

namespace MediatorSamples.ToggledRequestHandlers
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
                Toggle.Enabled = !Toggle.Enabled;
                Console.WriteLine();
            }
        }
    }

    [RequestHandlerToggle(typeof(Handler1), ToggleName = "Handler1", ToggleDisabledHandlerType = typeof(Handler2))]
    internal class Request : IRequest<string> { }

    internal class Toggle : IRequestHandlerToggle
    {
        public static bool Enabled { get; set; }

        public Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerToggleInfo toggleInfo, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>
        {
            Console.WriteLine($"evaluating {toggleInfo.ToggleName}");
            return Task.FromResult(Enabled);
        }
    }

    internal class Handler1 : IRequestHandler<Request, string>
    {
        public Task<string> Handle(Request request, CancellationToken cancellationToken)
        {
            return Task.FromResult("handler 1");
        }
    }

    internal class Handler2 : IRequestHandler<Request, string>
    {
        public Task<string> Handle(Request request, CancellationToken cancellationToken)
        {
            return Task.FromResult("handler 2");
        }
    }
}