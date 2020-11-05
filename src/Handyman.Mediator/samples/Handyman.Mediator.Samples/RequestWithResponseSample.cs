using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Samples
{
    public class RequestWithResponseSample : ISample
    {
        public int Order => 10;
        public string Name => "Request with response";

        public async Task Run(IMediator mediator)
        {
            var request = new Echo { Text = "Hello world" };
            var response = await mediator.Send(request, CancellationToken.None);
            Console.WriteLine(response);
        }

        public class Echo : IRequest<string>
        {
            public string Text { get; set; }
        }

        public class EchoHandler : IRequestHandler<Echo, string>
        {
            public Task<string> Handle(Echo request, CancellationToken cancellationToken)
            {
                return Task.FromResult(request.Text);
            }
        }
    }
}