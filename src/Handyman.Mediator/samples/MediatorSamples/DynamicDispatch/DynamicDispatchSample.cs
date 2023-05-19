using Handyman.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSamples.DynamicDispatch;

public class DynamicDispatchSample : Sample
{
   /*
    * Sometimes there is a need to invoke mediator using reflection. While that is perfectly doable, it does take some coding.
    * To aid in this scenario there are three interfaces available; IDynamicMediator, IDynamicSender & IDynamicPublisher (plus their implementations).
    * These types all uses object as input & output and handles all type conversions for you.
    */

   public override async Task RunAsync(CancellationToken cancellationToken)
   {
      var dynamicMediator = ServiceProvider.GetRequiredService<IDynamicMediator>();

      object request = Activator.CreateInstance(typeof(Request))!;

      object? response = await dynamicMediator.Send(request, cancellationToken);

      Console.WriteLine(response);
   }
}

public class Request : IRequest<string>
{
}

public class EchoHandler : IRequestHandler<Request, string>
{
   public Task<string> Handle(Request request, CancellationToken cancellationToken)
   {
      return Task.FromResult(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
   }
}