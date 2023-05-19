using Handyman.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSamples.GenericFilter;

public class GenericFilterSample : Sample
{
   public override async Task RunAsync(CancellationToken cancellationToken)
   {
      var mediator = ServiceProvider.GetRequiredService<IMediator>();

      Console.WriteLine(await mediator.Send(new GetNumber(), cancellationToken));
      Console.WriteLine(await mediator.Send(new GetString(), cancellationToken));
   }
}

public class GetNumber : IRequest<long>
{
}

public class GetString : IRequest<string>
{
}

public class Handler : IRequestHandler<GetNumber, long>, IRequestHandler<GetString, string>
{
   public Task<long> Handle(GetNumber request, CancellationToken cancellationToken)
   {
      return Task.FromResult(DateTime.Now.Ticks);
   }

   public Task<string> Handle(GetString request, CancellationToken cancellationToken)
   {
      return Task.FromResult(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
   }
}

public class Filter<TRequest, TResponse> : IRequestFilter<TRequest, TResponse>
   where TRequest : IRequest<string>
{
   public async Task<TResponse> Execute(RequestContext<TRequest> requestContext, RequestFilterExecutionDelegate<TResponse> next)
   {
      Console.WriteLine($"Filter processing request of type {requestContext.Request.GetType().Name}.");
      return await next();
   }
}