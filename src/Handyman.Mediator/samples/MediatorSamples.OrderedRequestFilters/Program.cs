using Handyman.Mediator;
using Handyman.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MediatorSamples.OrderedRequestFilters
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
            Console.WriteLine("handler executing");
            return Task.FromResult(request.Message);
        }
    }

    internal class FirstFilter<TRequest, TResponse> : IRequestFilter<TRequest, TResponse>, IOrderedFilter
        where TRequest : IRequest<TResponse>
    {
        public int Order => int.MinValue;

        public Task<TResponse> Execute(RequestPipelineContext<TRequest> context, RequestFilterExecutionDelegate<TResponse> next)
        {
            try
            {
                Console.WriteLine("first filter pre processing");
                return next();
            }
            finally
            {
                Console.WriteLine("first filter post processing");
            }
        }
    }

    internal class LastFilter<TRequest, TResponse> : IRequestFilter<TRequest, TResponse>, IOrderedFilter
        where TRequest : IRequest<TResponse>
    {
        public int Order => int.MaxValue;

        public Task<TResponse> Execute(RequestPipelineContext<TRequest> context, RequestFilterExecutionDelegate<TResponse> next)
        {
            try
            {
                Console.WriteLine("last filter pre processing");
                return next();
            }
            finally
            {
                Console.WriteLine("last filter post processing");
            }
        }
    }

    internal class UnorderedFilter<TRequest, TResponse> : IRequestFilter<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        public Task<TResponse> Execute(RequestPipelineContext<TRequest> context, RequestFilterExecutionDelegate<TResponse> next)
        {
            try
            {
                Console.WriteLine("unordered filter pre processing");
                return next();
            }
            finally
            {
                Console.WriteLine("unordered filter post processing");
            }
        }
    }
}
