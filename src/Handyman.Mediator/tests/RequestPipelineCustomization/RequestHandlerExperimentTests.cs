using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Handyman.Mediator.RequestPipelineCustomization;
using Maestro;
using Shouldly;
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
                x.Add<IRequestHandler<Request, string>>().Instance(new BaselineHandler { Action = () => "experimentBaseline" });
                x.Add<IRequestHandler<Request, string>>().Instance(new ExperimentHandler { Action = () => "experiment", Delay = 100 });
                x.Add<IRequestHandlerExperimentObserver<Request, string>>().Instance(observer);
                x.Add<IExperimentToggle<Request>>().Instance(new Toggle<Request> { Enabled = true });
            });

            var mediator = new Mediator(type => container.TryGetService(type, out var service) ? service : null);

            var request = new Request();

            var response = await mediator.Send(request);

            response.ShouldBe("experimentBaseline");

            observer.Request.ShouldBe(request);

            var baseline = observer.Baseline;

            //baseline.Canceled.ShouldBeFalse();
            //baseline.Exception.ShouldBeNull();
            //baseline.Faulted.ShouldBeFalse();
            //baseline.Handler.GetType().ShouldBe(typeof(BaselineHandler));
            //baseline.RanToCompletion.ShouldBeTrue();
            //baseline.Response.ShouldBe("experimentBaseline");

            var experiment = observer.Experiments.Single();

            //experiment.Canceled.ShouldBeFalse();
            //experiment.Exception.ShouldBeNull();
            //experiment.Faulted.ShouldBeFalse();
            //experiment.Handler.GetType().ShouldBe(typeof(ExperimentHandler));
            //experiment.RanToCompletion.ShouldBeTrue();
            //experiment.Response.ShouldBe("experiment");
        }

        [Fact]
        public async Task ShouldSucceedEventIfExperimentFails()
        {
            var evaluator = new Observer();

            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, string>>().Instance(new BaselineHandler { Action = () => "experimentBaseline" });
                x.Add<IRequestHandler<Request, string>>().Instance(new ExperimentHandler { Action = () => throw new Exception() });
                x.Add<IRequestHandlerExperimentObserver<Request, string>>().Instance(evaluator);
                x.Add<IExperimentToggle<Request>>().Instance(new Toggle<Request> { Enabled = true });
            });

            var mediator = new Mediator(type => container.TryGetService(type, out var service) ? service : null);

            var request = new Request();

            var response = await mediator.Send(request);

            response.ShouldBe("experimentBaseline");

            var experiment = evaluator.Experiments.Single();

            //experiment.Task.Exception.ShouldNotBeNull();
            //experiment.Handler.GetType().ShouldBe(typeof(ExperimentHandler));
            //experiment.Task.Status.ShouldBe(TaskStatus.RanToCompletion);
            //experiment.Task.Response.ShouldBeNull("experiment");
        }

        [Fact]
        public async Task ShouldNotExecuteExperimentIfToggleIsDisabled()
        {
            var evaluator = new Observer();
            var toggle = new Toggle<Request>();

            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, string>>().Instance(new BaselineHandler { Action = () => "experimentBaseline" });
                x.Add<IRequestHandler<Request, string>>().Instance(new ExperimentHandler { Action = () => "experiment" });
                x.Add<IRequestHandlerExperimentObserver<Request, string>>().Instance(evaluator);
                x.Add<IExperimentToggle<Request>>().Instance(toggle);
            });

            var mediator = new Mediator(type => container.TryGetService(type, out var service) ? service : null);

            await mediator.Send(new Request());

            evaluator.Experiments.ShouldBeNull();
        }

        [RequestHandlerExperiment(typeof(BaselineHandler))]
        private class Request : IRequest<string> { }

        private class BaselineHandler : BaseHandler { }

        private class ExperimentHandler : BaseHandler { }

        private abstract class BaseHandler : IRequestHandler<Request, string>
        {
            public Func<string> Action { get; set; }
            public int Delay { get; set; }

            public async Task<string> Handle(Request request, CancellationToken cancellationToken)
            {
                await Task.Delay(Delay, cancellationToken);
                return Action.Invoke();
            }
        }

        private class Observer : IRequestHandlerExperimentObserver<Request, string>
        {
            public Request Request { get; set; }
            public RequestHandlerExperimentExecution<Request, string> Baseline { get; set; }
            public IReadOnlyCollection<RequestHandlerExperimentExecution<Request, string>> Experiments { get; set; }

            public Task Observe(RequestHandlerExperimentResult<Request, string> result)
            {
                Request = result.Request;
                Baseline = result.Baseline;
                Experiments = result.Experiments;

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