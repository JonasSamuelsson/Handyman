﻿using Handyman.Mediator.RequestPipelineCustomization;
using Maestro;
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

            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, string>>().Instance(new BaselineHandler { Action = () => "baseline" });
                x.Add<IRequestHandler<Request, string>>().Instance(new ExperimentHandler { Action = () => "experiment", Delay = 100 });
                x.Add<IRequestHandlerExperimentObserver<Request, string>>().Instance(observer);
                x.Add<IExperimentToggle<Request>>().Instance(new Toggle<Request> { Enabled = true });
            });

            var mediator = new Mediator(type => container.TryGetService(type, out var service) ? service : null);

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

            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, string>>().Instance(new BaselineHandler { Action = () => "baseline" });
                x.Add<IRequestHandler<Request, string>>().Instance(new ExperimentHandler { Action = () => throw new Exception("boom") });
                x.Add<IRequestHandlerExperimentObserver<Request, string>>().Instance(observer);
                x.Add<IExperimentToggle<Request>>().Instance(new Toggle<Request> { Enabled = true });
            });

            var mediator = new Mediator(type => container.TryGetService(type, out var service) ? service : null);

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
            var toggle = new Toggle<Request>();

            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, string>>().Instance(new BaselineHandler { Action = () => "baseline" });
                x.Add<IRequestHandler<Request, string>>().Instance(experimentHandler);
                x.Add<IRequestHandlerExperimentObserver<Request, string>>().Instance(observer);
                x.Add<IExperimentToggle<Request>>().Instance(toggle);
            });

            var mediator = new Mediator(type => container.TryGetService(type, out var service) ? service : null);

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

        private class Toggle<TRequest> : IExperimentToggle<TRequest>
        {
            public bool Enabled { get; set; }

            public Task<bool> IsEnabled(TRequest request, CancellationToken cancellationToken)
            {
                return Task.FromResult(Enabled);
            }
        }
    }
}