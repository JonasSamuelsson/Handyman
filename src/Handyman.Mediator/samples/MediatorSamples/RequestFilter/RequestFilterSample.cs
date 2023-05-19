using Handyman.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSamples.RequestFilter;

public class RequestFilterSample : Sample
{
   public override async Task RunAsync(CancellationToken cancellationToken)
   {
      var mediator = ServiceProvider.GetRequiredService<IMediator>();

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
      Console.WriteLine("Handler");
      return Task.FromResult(request.Text);
   }
}

public class EchoFilter : IRequestFilter<Echo, string>
{
   public async Task<string> Execute(RequestContext<Echo> requestContext, RequestFilterExecutionDelegate<string> next)
   {
      try
      {
         Console.WriteLine("Filter pre process");
         return await next();
      }
      finally
      {
         Console.WriteLine("Filter post process");
      }
   }
}