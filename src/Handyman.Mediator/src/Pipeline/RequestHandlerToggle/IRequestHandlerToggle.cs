using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline.RequestHandlerToggle
{
    public interface IRequestHandlerToggle
   {
      Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerToggleMetadata toggleMetadata,
          RequestContext<TRequest> requestContext)
          where TRequest : IRequest<TResponse>;
   }
}