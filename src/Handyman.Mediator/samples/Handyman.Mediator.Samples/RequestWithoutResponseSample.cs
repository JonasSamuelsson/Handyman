using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Samples
{
    public class RequestWithoutResponseSample : ISample
    {
        public int Order => 20;
        public string Name => "Request without response (command)";

        public async Task Run(IMediator mediator)
        {
            await mediator.Send(new ConsoleWriteLine { Message = "Hello world" }, CancellationToken.None);
        }

        public class ConsoleWriteLine : IRequest
        {
            public string Message { get; set; }
        }

        public class WriteHandler : SyncRequestHandler<ConsoleWriteLine>
        {
            protected override void Handle(ConsoleWriteLine request, CancellationToken cancellationToken)
            {
                Console.WriteLine(request.Message);
            }
        }
    }
}