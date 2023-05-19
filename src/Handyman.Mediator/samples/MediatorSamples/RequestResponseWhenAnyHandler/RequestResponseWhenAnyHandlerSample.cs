using Handyman.Mediator;
using Handyman.Mediator.Pipeline.WhenAnyRequestHandler;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSamples.RequestResponseWhenAnyHandler;

public class RequestResponseWhenAnyHandlerSample : Sample
{
   public override async Task RunAsync(CancellationToken cancellationToken)
   {
      var mediator = ServiceProvider.GetRequiredService<IMediator>();

      for (int i = 0; i < 10; i++)
      {
         Console.WriteLine(await mediator.Send(new GetString(), cancellationToken));
      }
   }
}

[WhenAnyRequestHandler]
public class GetString : IRequest<string>
{
}

public class Handler1 : IRequestHandler<GetString, string>
{
   public async Task<string> Handle(GetString request, CancellationToken cancellationToken)
   {
      await Task.Delay(new Random().Next(100), cancellationToken);
      return GetType().Name;
   }
}

public class Handler2 : IRequestHandler<GetString, string>
{
   public async Task<string> Handle(GetString request, CancellationToken cancellationToken)
   {
      await Task.Delay(new Random().Next(100), cancellationToken);
      return GetType().Name;
   }
}