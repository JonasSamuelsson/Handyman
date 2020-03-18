using Handyman.Mediator;
using Handyman.Mediator.DependencyInjection;
using Handyman.Mediator.RequestPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MediatorSamples.RequestHandlerExperiments
{
    public class Program
    {
        public static async Task Main()
        {
            var serviceProvider = new ServiceCollection()
                .AddMediator(options => options.ScanEntryAssembly())
                .BuildServiceProvider();

            var mediator = serviceProvider.GetRequiredService<IMediator>();

            for (var i = 0; i < 4; i++)
            {
                Console.WriteLine(await mediator.Send(new Request()));
                Console.WriteLine();
            }
        }

        [RequestHandlerExperiment(typeof(DefaultHandler))]
        public class Request : IRequest<string> { }

        internal class Toggle : IRequestHandlerExperimentToggle
        {
            private static int _counter;

            public Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerExperimentInfo experimentInfo, RequestPipelineContext<TRequest> context)
                where TRequest : IRequest<TResponse>
            {
                return Task.FromResult(_counter++ != 0);
            }
        }

        internal class Observer : IRequestHandlerExperimentObserver
        {
            public Task Observe<TRequest, TResponse>(RequestHandlerExperiment<TRequest, TResponse> experiment)
                where TRequest : IRequest<TResponse>
            {
                Console.WriteLine($"baseline result: {experiment.BaselineExecution.Task.Result}");

                var experimentExecution = experiment.ExperimentalExecutions.Single();
                var message = experimentExecution.Task.IsCompletedSuccessfully
                    ? $"experiment response: {experimentExecution.Task.Result}"
                    : $"experiment error: {experimentExecution.Task.Exception.Message}";
                Console.WriteLine(message);

                return Task.CompletedTask;
            }
        }

        internal class DefaultHandler : IRequestHandler<Request, string>
        {
            public Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                return Task.FromResult("Hello world");
            }
        }

        internal class ExperimentalHandler : IRequestHandler<Request, string>
        {
            private static int _counter;

            public Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                var value = _counter++ % 3;
                return value == 0
                    ? Task.FromResult("Hello world")
                    : value == 1
                        ? Task.FromResult("Bye bye")
                        : throw new Exception("fail");
            }
        }
    }
}
