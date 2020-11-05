using Handyman.Mediator.Pipeline.RequestHandlerExperiment;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Samples
{
    public class RequestHandlerExperimentSample : ISample
    {
        private static readonly Random Random = new Random();

        public int Order => 180;
        public string Name => "Request handler experiment";

        public async Task Run(IMediator mediator)
        {
            for (var i = 0; i < 5; i++)
            {
                await mediator.Send(new Request(), CancellationToken.None);
                Console.WriteLine();
            }
        }

        [RequestHandlerExperiment(typeof(FirstHandler))]
        public class Request : IRequest<string> { }

        public class FirstHandler : IRequestHandler<Request, string>
        {
            public async Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                await Task.Delay(Random.Next(100), cancellationToken);
                return "foo";
            }
        }

        public class SecondHandler : IRequestHandler<Request, string>
        {
            private static int _counter = 0;

            public async Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                await Task.Delay(Random.Next(100), cancellationToken);
                return (_counter++ % 2 == 0) ? "foo" : "bar";
            }
        }

        public class Toggle : IRequestHandlerExperimentToggle
        {
            public Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerExperimentMetadata experimentMetadata, RequestContext<TRequest> requestContext) where TRequest : IRequest<TResponse>
            {
                return Task.FromResult(true);
            }
        }

        public class Observer : IRequestHandlerExperimentObserver
        {
            public Task Observe<TRequest, TResponse>(RequestHandlerExperiment<TRequest, TResponse> experiment) where TRequest : IRequest<TResponse>
            {
                var baseline = experiment.BaselineExecution;
                Console.WriteLine($"baseline;   result: {baseline.Task.Result}, duration: {baseline.Duration.TotalMilliseconds}ms");

                var exp = experiment.ExperimentalExecutions.Single();
                Console.WriteLine($"experiment; result: {exp.Task.Result}, duration: {exp.Duration.TotalMilliseconds}ms");

                return Task.CompletedTask;
            }
        }
    }
}