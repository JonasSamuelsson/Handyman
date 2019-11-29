using System;
using System.Threading.Tasks;

namespace Handyman.Mediator.RequestPipelineCustomization
{
    public class RequestHandlerExperimentExecution<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        public TimeSpan Duration { get; set; }
        public IRequestHandler<TRequest, TResponse> Handler { get; set; }
        public Task<TResponse> Task { get; set; }
    }
}