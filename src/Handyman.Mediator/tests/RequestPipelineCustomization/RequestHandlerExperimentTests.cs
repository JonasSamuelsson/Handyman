using Handyman.Mediator.RequestPipelineCustomization;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Handyman.Mediator.Tests.RequestPipelineCustomization
{
    public class RequestHandlerExperimentTests
    {
        [Fact]
        public async Task ShouldRunExperiment()
        {
            var observer = new Observer();

            var services = new ServiceCollection();

            services.AddSingleton<IRequestHandler<Request, string>>(new BaselineHandler { Action = () => "baseline" });
            services.AddSingleton<IRequestHandler<Request, string>>(new ExperimentHandler { Action = () => "experiment", Delay = 100 });
            services.AddSingleton<IRequestHandlerExperimentObserver<Request, string>>(observer);
            services.AddSingleton<IRequestHandlerExperimentToggle>(new Toggle { Enabled = true });

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
        }

        [Fact]
        public async Task ShouldSucceedEvenIfExperimentHandlerFails()
        {
            var observer = new Observer();

            var services = new ServiceCollection();

            services.AddSingleton<IRequestHandler<Request, string>>(new BaselineHandler { Action = () => "baseline" });
            services.AddSingleton<IRequestHandler<Request, string>>(new ExperimentHandler { Action = () => throw new Exception("boom") });
            services.AddSingleton<IRequestHandlerExperimentObserver<Request, string>>(observer);
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
            services.AddSingleton<IRequestHandlerExperimentObserver<Request, string>>(observer);
            services.AddSingleton<IRequestHandlerExperimentToggle>(toggle);

            var mediator = new Mediator(services.BuildServiceProvider());

            await mediator.Send(new Request());

            experimentHandler.Executed.ShouldBeFalse();
            observer.Executed.ShouldBeFalse();
        }

        [RequestHandlerExperiment(typeof(BaselineHandler))]
        private class Request : IRequest<string> { }

        private class BaselineHandler : BaseHandler { }

        private class ExperimentHandler : BaseHandler { }

        private abstract class BaseHandler : IRequestHandler<Request, string>
        {
            public Func<string> Action { get; set; }
            public int Delay { get; set; }
            public bool Executed { get; set; }

            public async Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                Executed = true;
                await Task.Delay(Delay, cancellationToken);
                return Action.Invoke();
            }
        }

        private class Observer : IRequestHandlerExperimentObserver<Request, string>
        {
            public bool Executed { get; set; }
            public Request Request { get; set; }
            public RequestHandlerExperimentExecution<Request, string> Baseline { get; set; }
            public IReadOnlyCollection<RequestHandlerExperimentExecution<Request, string>> Experiments { get; set; }

            public Task Observe(RequestHandlerExperiment<Request, string> experiment)
            {
                Request = experiment.Request;
                Baseline = experiment.BaselineExecution;
                Experiments = experiment.ExperimentalExecutions;

                return Task.CompletedTask;
            }
        }

        private class Toggle : IRequestHandlerExperimentToggle
        {
            public bool Enabled { get; set; }

            public Task<bool> IsEnabled<TRequest, TResponse>(TRequest request, CancellationToken cancellationToken)
                where TRequest : IRequest<TResponse>
            {
                return Task.FromResult(Enabled);
            }
        }
    }
}