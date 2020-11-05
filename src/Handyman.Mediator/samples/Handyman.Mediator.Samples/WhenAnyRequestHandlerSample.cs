using Handyman.Mediator.Pipeline.WhenAnyRequestHandler;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Samples
{
    public class WhenAnyRequestHandlerSample : ISample
    {
        private static readonly Random Random = new Random();

        public int Order => 150;
        public string Name => "When any request handler";

        public async Task Run(IMediator mediator)
        {
            for (var i = 0; i < 6; i++)
            {
                Console.WriteLine($"Handled by {await mediator.Send(new Request(), CancellationToken.None)}");
                Console.WriteLine();
            }
        }

        [WhenAnyRequestHandler]
        public class Request : IRequest<string> { }

        public class FirstHandler : IRequestHandler<Request, string>
        {
            public async Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                var delay = Random.Next(100);
                Console.WriteLine($"first delay: {delay}");
                await Task.Delay(delay, cancellationToken);
                return "first";
            }
        }

        public class SecondHandler : IRequestHandler<Request, string>
        {
            public async Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                var delay = Random.Next(100);
                Console.WriteLine($"second delay: {delay}");
                await Task.Delay(delay, cancellationToken);
                return "second";
            }
        }
    }
}