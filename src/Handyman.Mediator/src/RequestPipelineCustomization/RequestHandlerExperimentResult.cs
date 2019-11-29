using System.Collections.Generic;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class RequestHandlerExperimentResult<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public TRequest Request { get; set; }
        public RequestHandlerExperimentExecution<TRequest, TResponse> Baseline { get; set; }
        public IReadOnlyCollection<RequestHandlerExperimentExecution<TRequest, TResponse>> Experiments { get; set; }
    }
}