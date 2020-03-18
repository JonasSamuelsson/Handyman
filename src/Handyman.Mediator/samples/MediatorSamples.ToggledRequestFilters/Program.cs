using Handyman.Mediator;
using Handyman.Mediator.DependencyInjection;
using Handyman.Mediator.RequestPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatorSamples.ToggledRequestFilters
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

    [RequestFilterToggle(typeof(Filter1), ToggleName = "Filter1", ToggleDisabledFilterType = typeof(Filter2))]
    [RequestFilterToggle(typeof(Filter3), ToggleName = "Filter3")]
    internal class Request : IRequest<string> { }

    internal class Toggle : IRequestFilterToggle
    {
        public static bool Enabled;

        public Task<bool> IsEnabled<TRequest, TResponse>(RequestFilterToggleInfo toggleInfo, RequestPipelineContext<TRequest> context)
            where TRequest : IRequest<TResponse>
        {
            Console.WriteLine($"evaluating {toggleInfo.ToggleName}");
            return Task.FromResult(Enabled);
        }
    }

    internal class Filter1 : IRequestFilter<Request, string>
    {
        public Task<string> Execute(RequestPipelineContext<Request> context, RequestFilterExecutionDelegate<string> next)
        {
            Console.WriteLine("filter 1");
            return next();
        }
    }

    internal class Filter2 : IRequestFilter<Request, string>
    {
        public Task<string> Execute(RequestPipelineContext<Request> context, RequestFilterExecutionDelegate<string> next)
        {
            Console.WriteLine("filter 2");
            return next();
        }
    }

    internal class Filter3 : IRequestFilter<Request, string>
    {
        public Task<string> Execute(RequestPipelineContext<Request> context, RequestFilterExecutionDelegate<string> next)
        {
            Console.WriteLine("filter 3");
            return next();
        }
    }

    internal class Handler : IRequestHandler<Request, string>
    {
        public Task<string> Handle(Request request, CancellationToken cancellationToken)
        {
            return Task.FromResult("Hello world");
        }
    }
}