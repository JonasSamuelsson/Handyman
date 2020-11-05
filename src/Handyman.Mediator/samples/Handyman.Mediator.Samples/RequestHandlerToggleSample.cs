using Handyman.Mediator.Pipeline;
using Handyman.Mediator.Pipeline.RequestHandlerToggle;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Handyman.Mediator.Samples
{
    public class RequestHandlerToggleSample : ISample
    {
        public int Order => 170;
        public string Name => "Toggled request handlers";

        public async Task Run(IMediator mediator)
        {
            for (var i = 0; i < 5; i++)
            {
                await mediator.Send(new Request(), CancellationToken.None);
            }
        }

        [RequestHandlerToggle(typeof(FirstHandler), ToggleDisabledHandlerTypes = new[] { typeof(SecondHandler) })]
        public class Request : IRequest { }

        public class FirstHandler : SyncRequestHandler<Request>
        {
            protected override void Handle(Request request, CancellationToken cancellationToken)
            {
                Console.WriteLine("Handled by first");
            }
        }

        public class SecondHandler : SyncRequestHandler<Request>
        {
            protected override void Handle(Request request, CancellationToken cancellationToken)
            {
                Console.WriteLine("Handled by second");
            }
        }

        public class Toggle : ToggleBase
        {
            private static int _counter = 0;

            public override Task<bool> IsEnabled(IToggleMetadata toggleMetadata, MessageContext pipelineContext)
            {
                var enabled = (_counter++ % 2 == 0);
                Console.WriteLine($"Toggle enabled: {enabled}");
                return Task.FromResult(enabled);
            }
        }
    }
}