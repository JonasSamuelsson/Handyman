using System.Collections.Generic;
using System.Threading;

namespace Handyman.Mediator.Pipeline.RequestHandlerExperiment
{
    public class RequestHandlerExperiment<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public RequestHandlerExperimentExecution<TRequest, TResponse> BaselineExecution { get; internal set; } = null!;
        public CancellationToken CancellationToken { get; internal set; }
        public IReadOnlyCollection<RequestHandlerExperimentExecution<TRequest, TResponse>> ExperimentalExecutions { get; internal set; } = null!;
        public TRequest Request { get; internal set; } = default!;
    }
}