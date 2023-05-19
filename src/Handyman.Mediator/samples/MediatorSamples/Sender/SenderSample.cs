using Handyman.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSamples.Sender;

public class SenderSample : Sample
{
   public override async Task RunAsync(CancellationToken cancellationToken)
   {
      var mediator = ServiceProvider.GetRequiredService<ISender<Echo, string>>();

      var request = new Echo { Text = "Hello" };

      var response = await mediator.Send(request, cancellationToken);

      Console.WriteLine(response);
   }
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