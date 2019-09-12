namespace Handyman.Mediator.Experiments
{
    public class Baseline<TRequest, TResponse> : Experiment<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        internal Baseline(Execution<TRequest, TResponse> execution)
            : base(execution)
        {
        }
    }
}