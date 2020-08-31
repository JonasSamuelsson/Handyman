using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.RequestHandlerExperiment
{
    public interface IRequestHandlerExperimentToggle
   {
      Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerExperimentMetadata experimentMetadata,
          RequestContext<TRequest> requestContext)
          where TRequest : IRequest<TResponse>;
   }
}