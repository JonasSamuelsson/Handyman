using System.Collections.Generic;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class RequestHandlerExperiment<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public TRequest Request { get; internal set; }
        public RequestHandlerExperimentExecution<TRequest, TResponse> BaselineExecution { get; internal set; }
        public IReadOnlyCollection<RequestHandlerExperimentExecution<TRequest, TResponse>> ExperimentalExecutions { get; internal set; }
    }
}