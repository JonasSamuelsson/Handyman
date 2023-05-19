using Handyman.Mediator;
using Handyman.Mediator.Pipeline.RequestHandlerExperiment;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace MediatorSamples.RequestResponseExperiment;

public class RequestResponseExperimentSample : Sample
{
   public override async Task RunAsync(CancellationToken cancellationToken)
   {
      var mediator = ServiceProvider.GetRequiredService<IMediator>();

      Console.WriteLine(await mediator.Send(new GetString(), cancellationToken));
   }
}

[RequestHandlerExperiment(typeof(Handler1), ExperimentalHandlerTypes = new[] { typeof(Handler2), typeof(Handler3) })]
public class GetString : IRequest<string>
{
}

public class Handler1 : IRequestHandler<GetString, string>
{
   public async Task<string> Handle(GetString request, CancellationToken cancellationToken)
   {
      await Task.Delay(new Random().Next(100), cancellationToken);
      return DateTime.Now.TimeOfDay.ToString();
   }
}

public class Handler2 : IRequestHandler<GetString, string>
{
   public async Task<string> Handle(GetString request, CancellationToken cancellationToken)
   {
      await Task.Delay(new Random().Next(100), cancellationToken);
      return DateTime.Now.TimeOfDay.ToString();
   }
}

public class Handler3 : IRequestHandler<GetString, string>
{
   public async Task<string> Handle(GetString request, CancellationToken cancellationToken)
   {
      await Task.Delay(new Random().Next(100), cancellationToken);
      throw new Exception("Oh no, this didn't work.");
   }
}

public class Toggle : IRequestHandlerExperimentToggle
{
   public async Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerExperimentMetadata experimentMetadata, RequestContext<TRequest> requestContext) where TRequest : IRequest<TResponse>
   {
      await Task.Yield();
      return true;
   }
}

public class Observer : IRequestHandlerExperimentObserver
{
   public Task Observe<TRequest, TResponse>(RequestHandlerExperiment<TRequest, TResponse> experiment) where TRequest : IRequest<TResponse>
   {
      var result = new
      {
         Request = new
         {
            Type = experiment.Request.GetType().FullName,
            Value = experiment.Request
         },
         Baseline = Map(experiment.BaselineExecution),
         Experiments = experiment.ExperimentalExecutions.Select(Map)
      };

      Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));

      return Task.CompletedTask;
   }

   private static object Map<TRequest, TResponse>(RequestHandlerExperimentExecution<TRequest, TResponse> execution) where TRequest : IRequest<TResponse>
   {
      var taskStatus = execution.Task.Status;

      return new
      {
         execution.Duration,
         HandlerType = execution.Handler.GetType().FullName,
         Status = taskStatus.ToString(),
         Result = taskStatus == TaskStatus.RanToCompletion
            ? (object?)execution.Task.Result
            : default,
         Exception = execution.Task.Exception != null
            ? $"{execution.Task.Exception.GetType().Name}: {execution.Task.Exception.Message}"
            : null
      };
   }
}