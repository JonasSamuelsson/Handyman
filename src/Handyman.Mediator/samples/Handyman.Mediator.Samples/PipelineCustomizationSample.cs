using Handyman.DependencyInjection;
using Handyman.Mediator.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Console = System.Console;

namespace Handyman.Mediator.Samples
{
    public class PipelineCustomizationSample : ISample
    {
        public int Order => 200;
        public string Name => "Pipeline customization";

        public async Task Run(IMediator mediator)
        {
            for (var i = 0; i < 5; i++)
            {
                try
                {
                    await mediator.Send(new Request(), CancellationToken.None);
                    Console.WriteLine("Success");
                }
                catch
                {
                    Console.WriteLine("Failure");
                }

                Console.WriteLine();
            }
        }

        [RetryRequestHandler(MaxRetries = 3)]
        public class Request : IRequest { }

        public class Handler : SyncRequestHandler<Request>
        {
            private static readonly Random Random = new Random();

            protected override void Handle(Request request, CancellationToken cancellationToken)
            {
                if (Random.Next(3) == 0)
                    return;

                throw new Exception();
            }
        }

        public class RetryRequestHandlerAttribute : RequestPipelineBuilderAttribute, IRequestHandlerSelector
        {
            public int MaxRetries { get; set; }

            public override void Configure(IRequestPipelineBuilder builder, IServiceProvider serviceProvider)
            {
                builder.AddHandlerSelector(this);
            }

            public Task SelectHandlers<TRequest, TResponse>(List<IRequestHandler<TRequest, TResponse>> handlers, RequestContext<TRequest> requestContext) where TRequest : IRequest<TResponse>
            {
                var retryHandler = new RetryHandler<TRequest, TResponse>
                {
                    MaxRetries = MaxRetries,
                    InnerHandler = handlers.Single()
                };

                handlers.Clear();
                handlers.Add(retryHandler);

                return Task.CompletedTask;
            }
        }

        [ScannerIgnore]
        public class RetryHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
        {
            public int MaxRetries { get; set; }
            public IRequestHandler<TRequest, TResponse> InnerHandler { get; set; }

            public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken)
            {
                var attempt = 1;

                while (true)
                {
                    try
                    {
                        Console.Write($"Attempt {attempt} ");
                        var response = await InnerHandler.Handle(request, cancellationToken);
                        Console.WriteLine("succeeded");
                        return response;
                    }
                    catch
                    {
                        Console.WriteLine("failed");

                        if (attempt++ == MaxRetries)
                            throw;
                    }
                }
            }
        }
    }
}