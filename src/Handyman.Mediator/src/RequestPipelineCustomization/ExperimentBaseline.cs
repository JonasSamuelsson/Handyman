using Handyman.Mediator.Internals;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class ExperimentBaseline<TRequest, TResponse> : Experiment<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        internal ExperimentBaseline(ExperimentExecution<TRequest, TResponse> experimentExecution)
            : base(experimentExecution)
        {
        }
    }
}