using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Handyman.Mediator.Experiments;
using Maestro;
using Shouldly;
using Xunit;

namespace Handyman.Mediator.Tests
{
    public class ExperimentTests
    {
        [Fact]
        public async Task ShouldRunExperiment()
        {
            var evaluator = new Evaluator();

            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, string>>().Instance(new BaselineHandler { Action = () => "baseline" });
                x.Add<IRequestHandler<Request, string>>().Instance(new ExperimentHandler { Action = () => "experiment", Delay = 100 });
                x.Add<IExperimentEvaluator<Request, string>>().Instance(evaluator);
            });

            var mediator = new Mediator(type => container.TryGetService(type, out var service) ? service : null);

            var request = new Request();

            var response = await mediator.Send(request);

            response.ShouldBe("baseline");

            evaluator.Request.ShouldBe(request);

            evaluator.Baseline.Canceled.ShouldBeFalse();
            evaluator.Baseline.Exception.ShouldBeNull();
            evaluator.Baseline.Faulted.ShouldBeFalse();
            evaluator.Baseline.Handler.GetType().ShouldBe(typeof(BaselineHandler));
            evaluator.Baseline.RanToCompletion.ShouldBeTrue();
            evaluator.Baseline.Response.ShouldBe("baseline");

            var experiment = evaluator.Experiments.Single();

            experiment.Canceled.ShouldBeFalse();
            experiment.Exception.ShouldBeNull();
            experiment.Faulted.ShouldBeFalse();
            experiment.Handler.GetType().ShouldBe(typeof(ExperimentHandler));
            experiment.RanToCompletion.ShouldBeTrue();
            experiment.Response.ShouldBe("experiment");
        }

        [Fact]
        public async Task ShouldSucceedEventIfExperimentFails()
        {
            var evaluator = new Evaluator();

            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, string>>().Instance(new BaselineHandler { Action = () => "baseline" });
                x.Add<IRequestHandler<Request, string>>().Instance(new ExperimentHandler { Action = () => throw new Exception() });
                x.Add<IExperimentEvaluator<Request, string>>().Instance(evaluator);
            });

            var mediator = new Mediator(type => container.TryGetService(type, out var service) ? service : null);

            var request = new Request();

            var response = await mediator.Send(request);

            response.ShouldBe("baseline");

            var experiment = evaluator.Experiments.Single();

            experiment.Canceled.ShouldBeFalse();
            experiment.Exception.ShouldNotBeNull();
            experiment.Faulted.ShouldBeTrue();
            experiment.Handler.GetType().ShouldBe(typeof(ExperimentHandler));
            experiment.RanToCompletion.ShouldBeFalse();
            experiment.Response.ShouldBeNull("experiment");
        }

        [Fact]
        public async Task ShouldNotExecuteExperimentIfToggleIsDisabled()
        {
            var evaluator = new Evaluator();
            var toggle = new Toggle<Request>();

            var container = new Container(x =>
            {
                x.Add<IRequestHandler<Request, string>>().Instance(new BaselineHandler { Action = () => "baseline" });
                x.Add<IRequestHandler<Request, string>>().Instance(new ExperimentHandler { Action = () => "experiment" });
                x.Add<IExperimentEvaluator<Request, string>>().Instance(evaluator);
                x.Add<IExperimentToggle<Request>>().Instance(toggle);
            });

            var mediator = new Mediator(type => container.TryGetService(type, out var service) ? service : null);

            foreach (var enabled in new[] { false, true })
            {
                toggle.Enabled = enabled;

                await mediator.Send(new Request());

                (evaluator.Experiments != null).ShouldBe(enabled);
            }
        }

        [Experiment(typeof(BaselineHandler))]
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

        private class Evaluator : IExperimentEvaluator<Request, string>
        {
            public Request Request { get; set; }
            public Baseline<Request, string> Baseline { get; set; }
            public List<Experiment<Request, string>> Experiments { get; set; }

            public Task Evaluate(Request request, Baseline<Request, string> baseline, IEnumerable<Experiment<Request, string>> experiments)
            {
                Request = request;
                Baseline = baseline;
                Experiments = experiments.ToList();

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