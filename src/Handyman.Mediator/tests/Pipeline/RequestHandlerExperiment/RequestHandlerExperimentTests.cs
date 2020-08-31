using Handyman.Mediator.Pipeline;
using Handyman.Mediator.Pipeline.RequestHandlerExperiment;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests.Pipeline.RequestHandlerExperiment
{
    public class RequestHandlerExperimentTests
   {
      [Theory]
      [InlineData(false, false)]
      [InlineData(false, true)]
      [InlineData(true, false)]
      [InlineData(true, true)]
      public async Task ShouldRunExperiment(bool asyncBaselineHandler, bool asyncExperimentHandler)
      {
         var observer = new Observer();
         var toggle = new Toggle { Enabled = true };

         var services = new ServiceCollection();

         services.AddSingleton<IRequestHandler<Request, string>>(new BaselineHandler { Action = () => "baseline", Async = asyncBaselineHandler });
         services.AddSingleton<IRequestHandler<Request, string>>(new ExperimentHandler { Action = () => "experiment", Async = asyncExperimentHandler });
         services.AddSingleton<IRequestHandlerExperimentObserver>(observer);
         services.AddSingleton<IRequestHandlerExperimentToggle>(toggle);

         var mediator = new Mediator(services.BuildServiceProvider());

         var request = new Request();

         var response = await mediator.Send(request);

         response.ShouldBe("baseline");

         observer.Request.ShouldBe(request);

         var baseline = observer.Baseline;

         baseline.Handler.GetType().ShouldBe(typeof(BaselineHandler));
         baseline.Task.Exception.ShouldBeNull();
         baseline.Task.Result.ShouldBe("baseline");
         baseline.Task.Status.ShouldBe(TaskStatus.RanToCompletion);

         var experiment = observer.Experiments.Single();

         experiment.Handler.GetType().ShouldBe(typeof(ExperimentHandler));
         experiment.Task.Exception.ShouldBeNull();
         experiment.Task.Result.ShouldBe("experiment");
         experiment.Task.Status.ShouldBe(TaskStatus.RanToCompletion);

         toggle.ExperimentMetadata.BaselineHandlerType.ShouldBe(typeof(BaselineHandler));
         toggle.ExperimentMetadata.Name.ShouldBe("test");
         toggle.ExperimentMetadata.Tags.ShouldBe(new[] { "1", "2" });
      }

      [Theory]
      [InlineData(false, false)]
      [InlineData(false, true)]
      [InlineData(true, false)]
      [InlineData(true, true)]
      public async Task ShouldSucceedEvenIfExperimentHandlerFails(bool asyncBaselineHandler, bool asyncExperimentHandler)
      {
         var observer = new Observer();

         var services = new ServiceCollection();

         services.AddSingleton<IRequestHandler<Request, string>>(new BaselineHandler { Action = () => "baseline", Async = asyncBaselineHandler });
         services.AddSingleton<IRequestHandler<Request, string>>(new ExperimentHandler { Action = () => throw new Exception("boom"), Async = asyncExperimentHandler });
         services.AddSingleton<IRequestHandlerExperimentObserver>(observer);
         services.AddSingleton<IRequestHandlerExperimentToggle>(new Toggle { Enabled = true });

         var mediator = new Mediator(services.BuildServiceProvider());

         var request = new Request();

         var response = await mediator.Send(request);

         response.ShouldBe("baseline");

         var experiment = observer.Experiments.Single();

         experiment.Handler.GetType().ShouldBe(typeof(ExperimentHandler));
         experiment.Task.Exception.ShouldBeOfType<AggregateException>();
         experiment.Task.Exception.InnerException.Message.ShouldBe("boom");
         experiment.Task.Status.ShouldBe(TaskStatus.Faulted);
      }

      [Fact]
      public async Task ShouldNotExecuteExperimentIfToggleIsDisabled()
      {
         var experimentHandler = new ExperimentHandler();
         var observer = new Observer();
         var toggle = new Toggle();

         var services = new ServiceCollection();

         services.AddSingleton<IRequestHandler<Request, string>>(new BaselineHandler { Action = () => "baseline" });
         services.AddSingleton<IRequestHandler<Request, string>>(experimentHandler);
         services.AddSingleton<IRequestHandlerExperimentObserver>(observer);
         services.AddSingleton<IRequestHandlerExperimentToggle>(toggle);

         var mediator = new Mediator(services.BuildServiceProvider());

         await mediator.Send(new Request());

         experimentHandler.Executed.ShouldBeFalse();
         observer.Executed.ShouldBeFalse();
      }

      [RequestHandlerExperiment(typeof(BaselineHandler), Name = "test", Tags = new[] { "1", "2" })]
      private class Request : IRequest<string> { }

      private class BaselineHandler : BaseHandler { }

      private class ExperimentHandler : BaseHandler { }

      private abstract class BaseHandler : IRequestHandler<Request, string>
      {
         public Func<string> Action { get; set; }
         public bool Async { get; set; }
         public bool Executed { get; set; }

         public Task<string> Handle(Request request, CancellationToken cancellationToken)
         {
            Executed = true;
            return Async ? ExecuteAsync() : Execute();
         }

         private async Task<string> ExecuteAsync()
         {
            await Task.Delay(100);
            return Action();
         }

         private Task<string> Execute()
         {
            return Task.FromResult(Action());
         }
      }

      private class Observer : IRequestHandlerExperimentObserver
      {
         public bool Executed { get; set; }
         public Request Request { get; set; }
         public RequestHandlerExperimentExecution<Request, string> Baseline { get; set; }
         public IReadOnlyCollection<RequestHandlerExperimentExecution<Request, string>> Experiments { get; set; }

         public Task Observe<TRequest, TResponse>(RequestHandlerExperiment<TRequest, TResponse> experiment)
             where TRequest : IRequest<TResponse>
         {
            Request = (Request)(object)experiment.Request;
            Baseline = (RequestHandlerExperimentExecution<Request, string>)(object)experiment.BaselineExecution;
            Experiments = experiment.ExperimentalExecutions.Cast<RequestHandlerExperimentExecution<Request, string>>().ToList();

            return Task.CompletedTask;
         }
      }

      private class Toggle : IRequestHandlerExperimentToggle
      {
         public bool Enabled { get; set; }
         public RequestHandlerExperimentMetadata ExperimentMetadata { get; set; }

         public Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerExperimentMetadata experimentMetadata,
             RequestContext<TRequest> requestContext)
             where TRequest : IRequest<TResponse>
         {
            ExperimentMetadata = experimentMetadata;
            return Task.FromResult(Enabled);
         }
      }
   }
}